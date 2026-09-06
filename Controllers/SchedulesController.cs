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

        public SchedulesController(IGameEventScheduleService service) => _service = service;

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
            return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, schedule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GameEventScheduleWriteDto dto) =>
            await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
