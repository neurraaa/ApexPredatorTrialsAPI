using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IGameEventScheduleService
    {
        Task<List<GameEventScheduleDto>> GetAllAsync();
        Task<GameEventScheduleDto?> GetByIdAsync(int id);
        Task<GameEventScheduleDto> CreateAsync(GameEventScheduleWriteDto dto);
        Task<bool> UpdateAsync(int id, GameEventScheduleWriteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
