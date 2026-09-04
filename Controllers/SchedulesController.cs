using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    public class SchedulesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SchedulesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameEventScheduleDto>>> GetAll()
        {
            var schedules = await _context.Schedules.ToListAsync();
            return Ok(_mapper.Map<List<GameEventScheduleDto>>(schedules));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameEventScheduleDto>> GetById(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Match)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule is null) return NotFound();
            return _mapper.Map<GameEventScheduleDto>(schedule);
        }

        [HttpPost]
        public async Task<ActionResult<GameEventScheduleDto>> Create(GameEventScheduleWriteDto dto)
        {
            var schedule = _mapper.Map<GameEventSchedule>(dto);
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, _mapper.Map<GameEventScheduleDto>(schedule));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GameEventScheduleWriteDto dto)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule is null) return NotFound();

            _mapper.Map(dto, schedule);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule is null) return NotFound();

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
