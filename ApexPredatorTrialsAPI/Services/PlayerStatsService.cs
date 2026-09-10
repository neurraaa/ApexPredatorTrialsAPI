using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class PlayerStatsService : IPlayerStatsService
    {
        private readonly IPlayerStatsRepository _repository;
        private readonly IMapper _mapper;

        public PlayerStatsService(IPlayerStatsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PlayerStatsDto>> GetAllAsync() =>
            _mapper.Map<List<PlayerStatsDto>>(await _repository.GetAllAsync());

        public async Task<PlayerStatsDto?> GetByIdAsync(int id)
        {
            var stats = await _repository.GetByIdAsync(id);
            return stats is null ? null : _mapper.Map<PlayerStatsDto>(stats);
        }

        public async Task<PlayerStatsDto> CreateAsync(PlayerStatsWriteDto dto)
        {
            var stats = _mapper.Map<PlayerStats>(dto);
            await _repository.AddAsync(stats);
            await _repository.SaveChangesAsync();
            return _mapper.Map<PlayerStatsDto>(stats);
        }

        public async Task<bool> UpdateAsync(int id, PlayerStatsWriteDto dto)
        {
            var stats = await _repository.GetByIdAsync(id);
            if (stats is null) return false;

            _mapper.Map(dto, stats);
            _repository.Update(stats);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var stats = await _repository.GetByIdAsync(id);
            if (stats is null) return false;

            _repository.Delete(stats);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
