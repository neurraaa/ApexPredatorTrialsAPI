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
        private readonly IGameMapRepository _mapRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchResultsService _matchResultsService;
        private readonly IMapper _mapper;

        public GameEventService(
            IGameEventRepository repository,
            IGameEventScheduleRepository scheduleRepository,
            IGameEventRegistrationRepository registrationRepository,
            IPlayerRepository playerRepository,
            IPlayerStatsRepository statsRepository,
            IGameMapRepository mapRepository,
            IMatchRepository matchRepository,
            IMatchResultsService matchResultsService,
            IMapper mapper)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
            _registrationRepository = registrationRepository;
            _playerRepository = playerRepository;
            _statsRepository = statsRepository;
            _mapRepository = mapRepository;
            _matchRepository = matchRepository;
            _matchResultsService = matchResultsService;
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

            var requiredMatchupCount = TournamentRound.RequiredStartingMatchupCount(dto.StartingRound)!.Value;
            if (dto.Matchups.Count != requiredMatchupCount)
                return ServiceResult<GameEventDto>.Invalid($"{dto.StartingRound} requires exactly {requiredMatchupCount} matchups.");

            var resolved = new List<(MatchupCreateDto Matchup, Player? HunterExisting, Player? HumanExisting)>();

            foreach (var matchup in dto.Matchups)
            {
                if (matchup.Hunter.PlayerId is not null && matchup.Hunter.PlayerId == matchup.Human.PlayerId)
                    return ServiceResult<GameEventDto>.Invalid("A player cannot compete against themselves in the same matchup.");

                if (await _mapRepository.GetByIdAsync(matchup.MapId) is null)
                    return ServiceResult<GameEventDto>.Invalid($"Map {matchup.MapId} does not exist.");

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

                var match = new Match
                {
                    Event = ev,
                    HunterPlayer = hunter,
                    HumanPlayer = human,
                    MapId = matchup.MapId,
                    Region = dto.Region,
                    DateTime = DateTime.UtcNow
                };
                await _matchRepository.AddAsync(match);

                var schedule = new GameEventSchedule
                {
                    Event = ev,
                    Round = dto.StartingRound,
                    Match = match
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

            var slotsById = currentSlots.ToDictionary(s => s.Id);

            var winnerBySlotId = new Dictionary<int, int>();
            foreach (var slot in currentSlots)
            {
                var winnerId = slot.Match?.MatchResults?.WinnerId;
                if (winnerId is null)
                    return ServiceResult<GameEventDto>.Invalid($"Schedule slot {slot.Id} does not have a recorded match result yet.");

                winnerBySlotId[slot.Id] = winnerId.Value;
            }

            var usedSlotIds = new HashSet<int>();
            foreach (var matchup in dto.NextMatchups)
            {
                if (!usedSlotIds.Add(matchup.HunterScheduleId) || !usedSlotIds.Add(matchup.HumanScheduleId))
                    return ServiceResult<GameEventDto>.Invalid("Each current-round winner can only advance into one next-round matchup.");
            }
            if (!usedSlotIds.SetEquals(slotsById.Keys))
                return ServiceResult<GameEventDto>.Invalid("Every current-round matchup's winner must advance into exactly one next-round matchup.");

            foreach (var matchup in dto.NextMatchups)
            {
                if (await _mapRepository.GetByIdAsync(matchup.MapId) is null)
                    return ServiceResult<GameEventDto>.Invalid($"Map {matchup.MapId} does not exist.");
            }

            foreach (var matchup in dto.NextMatchups)
            {
                var hunterSlot = slotsById[matchup.HunterScheduleId];
                var humanSlot = slotsById[matchup.HumanScheduleId];

                var nextMatch = new Match
                {
                    Event = ev,
                    HunterPlayerId = winnerBySlotId[hunterSlot.Id],
                    HumanPlayerId = winnerBySlotId[humanSlot.Id],
                    MapId = matchup.MapId,
                    Region = ev.Region,
                    DateTime = DateTime.UtcNow
                };
                await _matchRepository.AddAsync(nextMatch);

                var nextSlot = new GameEventSchedule
                {
                    Event = ev,
                    Round = dto.NextRound,
                    Match = nextMatch
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

        public async Task<ServiceResult<GameEventScheduleDto>> SetMatchResultAsync(int eventId, int scheduleId, ScheduleMatchResultDto dto)
        {
            var slot = await _scheduleRepository.GetByIdWithMatchAsync(scheduleId);
            if (slot is null || slot.EventId != eventId)
                return ServiceResult<GameEventScheduleDto>.NotFound();

            if (slot.MatchId is null)
                return ServiceResult<GameEventScheduleDto>.Invalid("This matchup does not have an associated match yet.");

            var resultDto = new MatchResultsCreateDto
            {
                MatchId = slot.MatchId.Value,
                WinnerId = dto.WinnerId,
                LoserId = dto.LoserId,
                WinnerDeaths = dto.WinnerDeaths,
                WinnerKills = dto.WinnerKills,
                LoserDeaths = dto.LoserDeaths,
                LoserKills = dto.LoserKills,
                NestsDestroyed = dto.NestsDestroyed
            };

            var existing = await _matchResultsService.GetByMatchIdAsync(slot.MatchId.Value);
            var result = existing is null
                ? await _matchResultsService.CreateAsync(resultDto)
                : await _matchResultsService.UpdateAsync(existing.Id, resultDto);

            if (result.Status == ServiceStatus.NotFound)
                return ServiceResult<GameEventScheduleDto>.NotFound();
            if (result.Status == ServiceStatus.Invalid)
                return ServiceResult<GameEventScheduleDto>.Invalid(result.Error!);

            var refreshedSlot = await _scheduleRepository.GetByIdWithMatchAsync(scheduleId);
            return ServiceResult<GameEventScheduleDto>.Ok(_mapper.Map<GameEventScheduleDto>(refreshedSlot));
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
