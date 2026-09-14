using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(UserRegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result.Status == ServiceStatus.Invalid)
            {
                _logger.LogWarning("Registration rejected for {Username}: {Reason}", dto.Username, result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("User {UserId} registered ({Username})", result.Data!.User.Id, result.Data.User.Username);
            return Ok(result.Data);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result.Status == ServiceStatus.Invalid)
            {
                _logger.LogWarning("Login failed for username {Username}", dto.Username);
                return Unauthorized(result.Error);
            }

            _logger.LogInformation("User {UserId} logged in ({Username})", result.Data!.User.Id, result.Data.User.Username);
            return Ok(result.Data);
        }
    }
}
