using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _repository;
        private readonly IMapper _mapper;

        public PlayerService(IPlayerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PlayerDto>> GetAllAsync() =>
            _mapper.Map<List<PlayerDto>>(await _repository.GetAllWithStatsAsync());

        public async Task<PlayerDto?> GetByIdAsync(int id)
        {
            var player = await _repository.GetByIdWithStatsAsync(id);
            return player is null ? null : _mapper.Map<PlayerDto>(player);
        }

        public async Task<PlayerDetailDto?> GetDetailByIdAsync(int id)
        {
            var player = await _repository.GetByIdWithDetailsAsync(id);
            return player is null ? null : _mapper.Map<PlayerDetailDto>(player);
        }

        public async Task<PlayerDto?> GetByUserIdAsync(int userId)
        {
            var player = await _repository.GetByUserIdAsync(userId);
            return player is null ? null : _mapper.Map<PlayerDto>(player);
        }

        public async Task<List<PlayerDto>> GetByRegionAsync(string region) =>
            _mapper.Map<List<PlayerDto>>(await _repository.GetByRegionAsync(region));

        public async Task<PlayerDto> CreateAsync(PlayerWriteDto dto)
        {
            var player = _mapper.Map<Player>(dto);
            await _repository.AddAsync(player);
            await _repository.SaveChangesAsync();
            return _mapper.Map<PlayerDto>(player);
        }

        public async Task<bool> UpdateAsync(int id, PlayerWriteDto dto)
        {
            var player = await _repository.GetByIdAsync(id);
            if (player is null) return false;

            _mapper.Map(dto, player);
            _repository.Update(player);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var player = await _repository.GetByIdAsync(id);
            if (player is null) return false;

            _repository.Delete(player);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
