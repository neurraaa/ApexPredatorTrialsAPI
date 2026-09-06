using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IGameMapService
    {
        Task<List<GameMapDto>> GetAllAsync();
        Task<GameMapDto?> GetByIdAsync(int id);
        Task<GameMapDto> CreateAsync(GameMapWriteDto dto);
        Task<bool> UpdateAsync(int id, GameMapWriteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
