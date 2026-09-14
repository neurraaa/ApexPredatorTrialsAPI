using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Services;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IGameEventService
    {
        Task<List<GameEventDto>> GetAllAsync();
        Task<GameEventDto?> GetByIdAsync(int id);
        Task<List<GameEventScheduleDto>> GetBracketAsync(int eventId);
        Task<ServiceResult<GameEventDto>> CreateAsync(GameEventCreateDto dto, int organizerId);
        Task<bool> UpdateAsync(int id, GameEventWriteDto dto);
        Task<bool> DeleteAsync(int id);
        Task<ServiceResult<GameEventDto>> AdvancePhaseAsync(int eventId, AdvancePhaseDto dto);
        Task<ServiceResult<GameEventScheduleDto>> ConcludeAsync(int eventId, ConcludeEventDto dto);
    }
}
