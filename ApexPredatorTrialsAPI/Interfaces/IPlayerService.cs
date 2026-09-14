using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IPlayerService
    {
        Task<List<PlayerDto>> GetAllAsync();
        Task<PlayerDto?> GetByIdAsync(int id);
        Task<PlayerDetailDto?> GetDetailByIdAsync(int id);
        Task<PlayerDto?> GetByUserIdAsync(int userId);
        Task<List<PlayerDto>> GetByRegionAsync(string region);
        Task<List<PlayerDto>> SearchAsync(string term);
        Task<PlayerDto> CreateAsync(PlayerWriteDto dto);
        Task<bool> UpdateAsync(int id, PlayerWriteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
