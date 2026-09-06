using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Services;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/event-registrations")]
    public class EventRegistrationsController : ControllerBase
    {
        private readonly IGameEventRegistrationService _service;

        public EventRegistrationsController(IGameEventRegistrationService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameEventRegistrationDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<GameEventRegistrationDto>> GetById(int id)
        {
            var reg = await _service.GetByIdAsync(id);
            return reg is null ? NotFound() : Ok(reg);
        }

        [HttpPost]
        public async Task<ActionResult<GameEventRegistrationDto>> Create(GameEventRegistrationCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result.Status == ServiceStatus.Invalid) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
