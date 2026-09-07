using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService service, ILogger<UsersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _service.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto dto)
        {
            var result = await _service.RegisterAsync(dto);
            if (result.Status == ServiceStatus.Invalid)
            {
                _logger.LogWarning("Registration rejected for {Username}: {Reason}", dto.Username, result.Error);

                return BadRequest(result.Error);
            }

            _logger.LogInformation("User ({UserId}) registered as {Username}", result.Data!.Id, result.Data.Username);

            return CreatedAtAction(nameof(GetUser), new { id = result.Data!.Id }, result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!await _service.DeleteAsync(id))
            {
                _logger.LogWarning("Delete failed: User ({UserId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("User ({UserId}) deleted", id);

            return NoContent();
        }
    }
}
