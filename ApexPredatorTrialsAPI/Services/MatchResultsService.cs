using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class MatchResultsService : IMatchResultsService
    {
        private readonly IMatchResultsRepository _repository;
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public MatchResultsService(IMatchResultsRepository repository, IMatchRepository matchRepository, IMapper mapper)
        {
            _repository = repository;
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<List<MatchResultsDto>> GetAllAsync() =>
            _mapper.Map<List<MatchResultsDto>>(await _repository.GetAllWithPlayersAsync());

        public async Task<MatchResultsDto?> GetByIdAsync(int id)
        {
            var result = await _repository.GetByIdWithPlayersAsync(id);
            return result is null ? null : _mapper.Map<MatchResultsDto>(result);
        }

        public async Task<ServiceResult<MatchResultsDto>> CreateAsync(MatchResultsCreateDto dto)
        {
            var match = await _matchRepository.GetByIdAsync(dto.MatchId);
            if (match is null)
                return ServiceResult<MatchResultsDto>.NotFound();

            var validPlayerIds = new[] { match.HunterPlayerId, match.HumanPlayerId };
            if (!validPlayerIds.Contains(dto.WinnerId) || !validPlayerIds.Contains(dto.LoserId))
                return ServiceResult<MatchResultsDto>.Invalid("Winner and Loser must be the two players from the referenced Match.");
            if (dto.WinnerId == dto.LoserId)
                return ServiceResult<MatchResultsDto>.Invalid("Winner and Loser cannot be the same player.");

            var result = _mapper.Map<MatchResults>(dto);
            result.DateTimeConcluded = DateTime.UtcNow;

            await _repository.AddAsync(result);
            await _repository.SaveChangesAsync();

            var withPlayers = await _repository.GetByIdWithPlayersAsync(result.Id);
            return ServiceResult<MatchResultsDto>.Ok(_mapper.Map<MatchResultsDto>(withPlayers));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result is null) return false;

            _repository.Delete(result);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
