using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    public class SchedulesController : ControllerBase
    {
        private readonly IGameEventScheduleService _service;
        private readonly ILogger<SchedulesController> _logger;

        public SchedulesController(IGameEventScheduleService service, ILogger<SchedulesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameEventScheduleDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<GameEventScheduleDto>> GetById(int id)
        {
            var schedule = await _service.GetByIdAsync(id);
            return schedule is null ? NotFound() : Ok(schedule);
        }

        [HttpPost]
        public async Task<ActionResult<GameEventScheduleDto>> Create(GameEventScheduleWriteDto dto)
        {
            var schedule = await _service.CreateAsync(dto);

            _logger.LogInformation("Schedule ({ScheduleId}) created for Event ({EventId}) ({Round})",
            schedule.Id, schedule.EventId, schedule.Round);

            return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, schedule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GameEventScheduleWriteDto dto)
        {
            if (!await _service.UpdateAsync(id, dto))
            {
                _logger.LogWarning("Update failed: Schedule ({ScheduleId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Schedule ({ScheduleId}) updated", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.DeleteAsync(id))
            {
                _logger.LogWarning("Delete failed: Schedule ({ScheduleId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Schedule ({ScheduleId}) deleted", id);

            return NoContent();
        }
    }
}
