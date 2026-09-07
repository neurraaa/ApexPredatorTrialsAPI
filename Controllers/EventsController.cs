using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
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
        public async Task<ActionResult<IEnumerable<GameEventDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<GameEventDto>> GetById(int id)
        {
            var ev = await _service.GetByIdAsync(id);
            return ev is null ? NotFound() : Ok(ev);
        }

        [HttpGet("{id}/bracket")]
        public async Task<ActionResult<IEnumerable<GameEventScheduleDto>>> GetBracket(int id) =>
            Ok(await _service.GetBracketAsync(id));

        [HttpPost]
        public async Task<ActionResult<GameEventDto>> Create(GameEventWriteDto dto)
        {
            var ev = await _service.CreateAsync(dto);

            _logger.LogInformation("Event ({EventId}) created ({Title})", ev.Id, ev.Title);
            
            return CreatedAtAction(nameof(GetById), new { id = ev.Id }, ev);
        }

        [HttpPut("{id}")]
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
    }
}
