using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Services;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
