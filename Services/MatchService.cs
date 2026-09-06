using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _repository;
        private readonly IMapper _mapper;

        public MatchService(IMatchRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<MatchDto>> GetAllAsync() =>
            _mapper.Map<List<MatchDto>>(await _repository.GetAllWithDetailsAsync());

        public async Task<MatchDto?> GetByIdAsync(int id)
        {
            var match = await _repository.GetByIdWithDetailsAsync(id);
            return match is null ? null : _mapper.Map<MatchDto>(match);
        }

        public async Task<ServiceResult<MatchDto>> CreateAsync(MatchWriteDto dto)
        {
            if (dto.HunterPlayerId == dto.HumanPlayerId)
                return ServiceResult<MatchDto>.Invalid("A player cannot play both sides of the same match.");

            var match = _mapper.Map<Match>(dto);
            await _repository.AddAsync(match);
            await _repository.SaveChangesAsync();

            var withDetails = await _repository.GetByIdWithDetailsAsync(match.Id);
            return ServiceResult<MatchDto>.Ok(_mapper.Map<MatchDto>(withDetails));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, MatchWriteDto dto)
        {
            if (dto.HunterPlayerId == dto.HumanPlayerId)
                return ServiceResult<bool>.Invalid("A player cannot play both sides of the same match.");

            var match = await _repository.GetByIdAsync(id);
            if (match is null) return ServiceResult<bool>.NotFound();

            _mapper.Map(dto, match);
            _repository.Update(match);
            await _repository.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var match = await _repository.GetByIdAsync(id);
            if (match is null) return false;

            _repository.Delete(match);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
