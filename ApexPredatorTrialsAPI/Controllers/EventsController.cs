using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IGameEventService _service;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IGameEventService service, ILogger<EventsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GameEventDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<GameEventDto>> GetById(int id)
        {
            var ev = await _service.GetByIdAsync(id);

            return ev is null ? NotFound() : Ok(ev);
        }

        [HttpGet("{id}/bracket")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GameEventScheduleDto>>> GetBracket(int id) =>
            Ok(await _service.GetBracketAsync(id));

        [HttpPost]
        public async Task<ActionResult<GameEventDto>> Create(GameEventCreateDto dto)
        {
            var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim is null || !int.TryParse(userIdClaim, out var organizerId))
                return Unauthorized();

            var result = await _service.CreateAsync(dto, organizerId);

            if (result.Status == ServiceStatus.Invalid)
            {
                _logger.LogWarning("Failed to create event: {Error}", result.Error);

                return BadRequest(result.Error);
            }

            _logger.LogInformation("Event {EventId} created ({Title}) by User {OrganizerId}", result.Data!.Id, result.Data.Title, organizerId);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, GameEventWriteDto dto)
        {
            if (!await _service.UpdateAsync(id, dto))
            {
                _logger.LogWarning("Update failed: Event ({EventId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Event ({EventId}) updated", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.DeleteAsync(id))
            {
                _logger.LogWarning("Delete failed: Event ({EventId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Event ({EventId}) deleted", id);

            return NoContent();
        }

        [HttpPost("{id}/advance-phase")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GameEventDto>> AdvancePhase(int id, AdvancePhaseDto dto)
        {
            var result = await _service.AdvancePhaseAsync(id, dto);

            switch (result.Status)
            {
                case ServiceStatus.NotFound:
                    _logger.LogWarning("Advance phase failed: Event ({EventId}) not found", id);

                    return NotFound();
                case ServiceStatus.Invalid:
                    _logger.LogWarning("Advance phase failed for Event ({EventId}): {Error}", id, result.Error);

                    return BadRequest(result.Error);
                default:
                    _logger.LogInformation("Event ({EventId}) advanced to round {Round}", id, result.Data!.CurrentRound);

                    return Ok(result.Data);
            }
        }
    }
}
