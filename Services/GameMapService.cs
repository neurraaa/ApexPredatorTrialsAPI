using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class GameMapService : IGameMapService
    {
        private readonly IGameMapRepository _repository;
        private readonly IMapper _mapper;

        public GameMapService(IGameMapRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<GameMapDto>> GetAllAsync() =>
            _mapper.Map<List<GameMapDto>>(await _repository.GetAllAsync());

        public async Task<GameMapDto?> GetByIdAsync(int id)
        {
            var map = await _repository.GetByIdAsync(id);
            return map is null ? null : _mapper.Map<GameMapDto>(map);
        }

        public async Task<GameMapDto> CreateAsync(GameMapWriteDto dto)
        {
            var map = _mapper.Map<GameMap>(dto);
            await _repository.AddAsync(map);
            await _repository.SaveChangesAsync();
            return _mapper.Map<GameMapDto>(map);
        }

        public async Task<bool> UpdateAsync(int id, GameMapWriteDto dto)
        {
            var map = await _repository.GetByIdAsync(id);
            if (map is null) return false;

            _mapper.Map(dto, map);
            _repository.Update(map);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var map = await _repository.GetByIdAsync(id);
            if (map is null) return false;

            _repository.Delete(map);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
