using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IPlayerService
    {
        Task<List<PlayerDto>> GetAllAsync();
        Task<PlayerDto?> GetByIdAsync(int id);
        Task<List<PlayerDto>> GetByRegionAsync(string region);
        Task<PlayerDto> CreateAsync(PlayerWriteDto dto);
        Task<bool> UpdateAsync(int id, PlayerWriteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
