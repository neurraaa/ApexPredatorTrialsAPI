using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Services;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponseDto>> RegisterAsync(UserRegisterDto dto);
        Task<ServiceResult<AuthResponseDto>> LoginAsync(UserLoginDto dto);
    }
}
