using ApexPredatorTrialsAPI.Services;
using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IGameEventRegistrationService
    {
        Task<List<GameEventRegistrationDto>> GetAllAsync();
        Task<GameEventRegistrationDto?> GetByIdAsync(int id);
        Task<ServiceResult<GameEventRegistrationDto>> CreateAsync(GameEventRegistrationCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
