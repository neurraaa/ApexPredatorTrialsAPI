using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/event-registrations")]
    [Authorize]
    public class EventRegistrationsController : ControllerBase
    {
        private readonly IGameEventRegistrationService _service;
        private readonly ILogger<EventRegistrationsController> _logger;

        public EventRegistrationsController(IGameEventRegistrationService service, ILogger<EventRegistrationsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GameEventRegistrationDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<GameEventRegistrationDto>> GetById(int id)
        {
            var reg = await _service.GetByIdAsync(id);
            return reg is null ? NotFound() : Ok(reg);
        }

        [HttpPost]
        public async Task<ActionResult<GameEventRegistrationDto>> Create(GameEventRegistrationCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);

            if (result.Status == ServiceStatus.Invalid)
            {
                _logger.LogWarning("Registration rejected: Player ({PlayerId}) for Event ({EventId}): {Reason}",
                    dto.PlayerId, dto.EventId, result.Error);

                return BadRequest(result.Error);
            }

            _logger.LogInformation("Player ({PlayerId}) registered for Event ({EventId})", dto.PlayerId, dto.EventId);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.DeleteAsync(id))
            {
                _logger.LogWarning("Delete failed: EventRegistration ({RegistrationId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("EventRegistration ({RegistrationId}) deleted", id);

            return NoContent();
        }
    }
}
