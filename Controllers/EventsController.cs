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

        public EventsController(IGameEventService service) => _service = service;

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
            return CreatedAtAction(nameof(GetById), new { id = ev.Id }, ev);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GameEventWriteDto dto) =>
            await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
