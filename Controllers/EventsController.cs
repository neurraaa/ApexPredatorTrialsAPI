using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EventsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameEventDto>>> GetAll()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(_mapper.Map<List<GameEventDto>>(events));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameEventDto>> GetById(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev is null) return NotFound();
            return _mapper.Map<GameEventDto>(ev);
        }

        [HttpGet("{id}/bracket")]
        public async Task<ActionResult<IEnumerable<GameEventScheduleDto>>> GetBracket(int id)
        {
            var schedules = await _context.Schedules
                .Where(s => s.EventId == id)
                .Include(s => s.Match)
                .ToListAsync();

            return Ok(_mapper.Map<List<GameEventScheduleDto>>(schedules));
        }

        [HttpPost]
        public async Task<ActionResult<GameEventDto>> Create(GameEventWriteDto dto)
        {
            var ev = _mapper.Map<GameEvent>(dto);
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = ev.Id }, _mapper.Map<GameEventDto>(ev));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GameEventWriteDto dto)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev is null) return NotFound();

            _mapper.Map(dto, ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev is null) return NotFound();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
