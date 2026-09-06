using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IGameEventService
    {
        Task<List<GameEventDto>> GetAllAsync();
        Task<GameEventDto?> GetByIdAsync(int id);
        Task<List<GameEventScheduleDto>> GetBracketAsync(int eventId);
        Task<GameEventDto> CreateAsync(GameEventWriteDto dto);
        Task<bool> UpdateAsync(int id, GameEventWriteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
