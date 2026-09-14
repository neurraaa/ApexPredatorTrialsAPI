using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class GameEventService : IGameEventService
    {
        private readonly IGameEventRepository _repository;
        private readonly IGameEventScheduleRepository _scheduleRepository;
        private readonly IGameEventRegistrationRepository _registrationRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerStatsRepository _statsRepository;
        private readonly IMapper _mapper;

        public GameEventService(
            IGameEventRepository repository,
            IGameEventScheduleRepository scheduleRepository,
            IGameEventRegistrationRepository registrationRepository,
            IPlayerRepository playerRepository,
            IPlayerStatsRepository statsRepository,
            IMapper mapper)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
            _registrationRepository = registrationRepository;
            _playerRepository = playerRepository;
            _statsRepository = statsRepository;
            _mapper = mapper;
        }

        public async Task<List<GameEventDto>> GetAllAsync() =>
            _mapper.Map<List<GameEventDto>>(await _repository.GetAllAsync());

        public async Task<GameEventDto?> GetByIdAsync(int id)
        {
            var ev = await _repository.GetByIdAsync(id);
            return ev is null ? null : _mapper.Map<GameEventDto>(ev);
        }

        public async Task<List<GameEventScheduleDto>> GetBracketAsync(int eventId) =>
            _mapper.Map<List<GameEventScheduleDto>>(await _scheduleRepository.GetByEventIdAsync(eventId));

        public async Task<ServiceResult<GameEventDto>> CreateAsync(GameEventCreateDto dto, int organizerId)
        {
            if (!TournamentRound.IsValid(dto.StartingRound) || dto.StartingRound == "Final")
                return ServiceResult<GameEventDto>.Invalid("Starting round must be Quarter-Final or Semi-Final.");

            // Validate every matchup up front, without mutating any tracked entity, so a validation
            // failure partway through can never leave earlier matchups' player/stat changes to be
            // flushed by TimingMiddleware's unconditional SaveChangesAsync after this request returns.
            var resolved = new List<(MatchupCreateDto Matchup, Player? HunterExisting, Player? HumanExisting)>();

            foreach (var matchup in dto.Matchups)
            {
                if (matchup.Hunter.PlayerId is not null && matchup.Hunter.PlayerId == matchup.Human.PlayerId)
                    return ServiceResult<GameEventDto>.Invalid("A player cannot compete against themselves in the same matchup.");

                var (hunterExisting, hunterError) = await ValidateEntryAsync(matchup.Hunter);
                if (hunterError is not null) return ServiceResult<GameEventDto>.Invalid(hunterError);

                var (humanExisting, humanError) = await ValidateEntryAsync(matchup.Human);
                if (humanError is not null) return ServiceResult<GameEventDto>.Invalid(humanError);

                resolved.Add((matchup, hunterExisting, humanExisting));
            }

            var ev = new GameEvent
            {
                Title = dto.Title,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Region = dto.Region,
                OrganizerId = organizerId,
                StartingRound = dto.StartingRound,
                CurrentRound = dto.StartingRound
            };

            var registeredPlayerIds = new HashSet<int>();
            var registrations = new List<GameEventRegistration>();

            foreach (var (matchup, hunterExisting, humanExisting) in resolved)
            {
                var hunter = await ApplyEntryAsync(matchup.Hunter, hunterExisting);
                var human = await ApplyEntryAsync(matchup.Human, humanExisting);

                var schedule = new GameEventSchedule
                {
                    Event = ev,
                    Round = dto.StartingRound,
                    HunterPlayer = hunter,
                    HumanPlayer = human
                };
                await _scheduleRepository.AddAsync(schedule);

                AddRegistration(registrations, registeredPlayerIds, ev, hunter, matchup.Hunter.PlayerId);
                AddRegistration(registrations, registeredPlayerIds, ev, human, matchup.Human.PlayerId);
            }

            await _repository.AddAsync(ev);
            foreach (var registration in registrations)
                await _registrationRepository.AddAsync(registration);

            await _repository.SaveChangesAsync();
            return ServiceResult<GameEventDto>.Ok(_mapper.Map<GameEventDto>(ev));
        }

        public async Task<bool> UpdateAsync(int id, GameEventWriteDto dto)
        {
            var ev = await _repository.GetByIdAsync(id);
            if (ev is null) return false;

            _mapper.Map(dto, ev);
            _repository.Update(ev);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ev = await _repository.GetByIdAsync(id);
            if (ev is null) return false;

            _repository.Delete(ev);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<ServiceResult<GameEventDto>> AdvancePhaseAsync(int eventId, AdvancePhaseDto dto)
        {
            var ev = await _repository.GetByIdAsync(eventId);
            if (ev is null) return ServiceResult<GameEventDto>.NotFound();

            var expectedNext = TournamentRound.NextAfter(ev.CurrentRound);
            if (expectedNext is null || dto.NextRound != expectedNext)
                return ServiceResult<GameEventDto>.Invalid(
                    expectedNext is null
                        ? "This event's bracket has already reached the Final."
                        : $"Next round must be '{expectedNext}'.");

            var currentSlots = await _scheduleRepository.GetByEventIdAndRoundAsync(eventId, ev.CurrentRound);
            if (currentSlots.Count == 0)
                return ServiceResult<GameEventDto>.Invalid("There are no matchups in the current round to advance.");

            if (dto.Winners.Count != currentSlots.Count ||
                dto.Winners.Select(w => w.ScheduleId).Distinct().Count() != currentSlots.Count)
                return ServiceResult<GameEventDto>.Invalid("A winner must be declared for every matchup in the current round, exactly once.");

            var slotsById = currentSlots.ToDictionary(s => s.Id);

            // Validate every winner and pairing before mutating any tracked entity (see the same
            // note in CreateAsync) — a slot's WinnerPlayerId is only ever set once the whole request
            // is known to succeed.
            var winnerBySlotId = new Dictionary<int, int>();
            foreach (var winner in dto.Winners)
            {
                if (!slotsById.TryGetValue(winner.ScheduleId, out var slot))
                    return ServiceResult<GameEventDto>.Invalid($"Schedule slot {winner.ScheduleId} is not part of the current round of this event.");

                if (winner.WinnerPlayerId != slot.HunterPlayerId && winner.WinnerPlayerId != slot.HumanPlayerId)
                    return ServiceResult<GameEventDto>.Invalid($"The declared winner for slot {winner.ScheduleId} must be one of its two competing players.");

                winnerBySlotId[slot.Id] = winner.WinnerPlayerId;
            }

            var usedSlotIds = new HashSet<int>();
            foreach (var matchup in dto.NextMatchups)
            {
                if (!usedSlotIds.Add(matchup.HunterScheduleId) || !usedSlotIds.Add(matchup.HumanScheduleId))
                    return ServiceResult<GameEventDto>.Invalid("Each current-round winner can only advance into one next-round matchup.");
            }
            if (!usedSlotIds.SetEquals(slotsById.Keys))
                return ServiceResult<GameEventDto>.Invalid("Every current-round matchup's winner must advance into exactly one next-round matchup.");

            foreach (var slot in currentSlots)
                slot.WinnerPlayerId = winnerBySlotId[slot.Id];

            foreach (var matchup in dto.NextMatchups)
            {
                var hunterSlot = slotsById[matchup.HunterScheduleId];
                var humanSlot = slotsById[matchup.HumanScheduleId];

                var nextSlot = new GameEventSchedule
                {
                    Event = ev,
                    Round = dto.NextRound,
                    HunterPlayerId = hunterSlot.WinnerPlayerId,
                    HumanPlayerId = humanSlot.WinnerPlayerId
                };
                await _scheduleRepository.AddAsync(nextSlot);

                hunterSlot.NextSchedule = nextSlot;
                humanSlot.NextSchedule = nextSlot;
            }

            ev.CurrentRound = dto.NextRound;
            _repository.Update(ev);

            await _repository.SaveChangesAsync();
            return ServiceResult<GameEventDto>.Ok(_mapper.Map<GameEventDto>(ev));
        }

        public async Task<ServiceResult<GameEventScheduleDto>> ConcludeAsync(int eventId, ConcludeEventDto dto)
        {
            var ev = await _repository.GetByIdAsync(eventId);
            if (ev is null) return ServiceResult<GameEventScheduleDto>.NotFound();

            if (ev.CurrentRound != "Final")
                return ServiceResult<GameEventScheduleDto>.Invalid("The event must reach the Final round before it can be concluded.");

            var finalSlots = await _scheduleRepository.GetByEventIdAndRoundAsync(eventId, "Final");
            if (finalSlots.Count != 1)
                return ServiceResult<GameEventScheduleDto>.Invalid("The Final round does not have exactly one matchup.");

            var finalSlot = finalSlots[0];
            if (dto.WinnerPlayerId != finalSlot.HunterPlayerId && dto.WinnerPlayerId != finalSlot.HumanPlayerId)
                return ServiceResult<GameEventScheduleDto>.Invalid("The declared champion must be one of the two Final competitors.");

            finalSlot.WinnerPlayerId = dto.WinnerPlayerId;
            await _scheduleRepository.SaveChangesAsync();

            return ServiceResult<GameEventScheduleDto>.Ok(_mapper.Map<GameEventScheduleDto>(finalSlot));
        }

        private async Task<(Player? ExistingPlayer, string? Error)> ValidateEntryAsync(EventPlayerEntryDto entry)
        {
            if (entry.PlayerId is not null)
            {
                var player = await _playerRepository.GetByIdWithStatsAsync(entry.PlayerId.Value);
                return player is null ? (null, $"Player {entry.PlayerId} not found.") : (player, null);
            }

            return string.IsNullOrWhiteSpace(entry.Name) ? (null, "A new player entry must include a name.") : (null, null);
        }

        private async Task<Player> ApplyEntryAsync(EventPlayerEntryDto entry, Player? existingPlayer)
        {
            if (existingPlayer is not null)
            {
                _mapper.Map(entry.Stats, existingPlayer.PlayerStats);
                return existingPlayer;
            }

            var stats = _mapper.Map<PlayerStats>(entry.Stats);
            await _statsRepository.AddAsync(stats);

            var newPlayer = new Player
            {
                Name = entry.Name!,
                Platform = entry.Platform ?? string.Empty,
                Region = entry.Region ?? string.Empty,
                PlayerStats = stats
            };
            await _playerRepository.AddAsync(newPlayer);

            return newPlayer;
        }

        private static void AddRegistration(List<GameEventRegistration> registrations, HashSet<int> registeredPlayerIds, GameEvent ev, Player player, int? existingPlayerId)
        {
            if (existingPlayerId is not null && !registeredPlayerIds.Add(existingPlayerId.Value))
                return;

            registrations.Add(new GameEventRegistration
            {
                Event = ev,
                Player = player,
                DateRegistered = DateTime.UtcNow
            });
        }
    }
}
