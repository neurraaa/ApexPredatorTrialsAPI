using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IPlayerStatsService
    {
        Task<List<PlayerStatsDto>> GetAllAsync();
        Task<PlayerStatsDto?> GetByIdAsync(int id);
        Task<PlayerStatsDto> CreateAsync(PlayerStatsWriteDto dto);
        Task<bool> UpdateAsync(int id, PlayerStatsWriteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
