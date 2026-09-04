using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/event-registrations")]
    public class EventRegistrationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EventRegistrationsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameEventRegistrationDto>>> GetAll()
        {
            var regs = await _context.EventRegistrations.ToListAsync();
            return Ok(_mapper.Map<List<GameEventRegistrationDto>>(regs));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameEventRegistrationDto>> GetById(int id)
        {
            var reg = await _context.EventRegistrations.FindAsync(id);
            if (reg is null) return NotFound();
            return _mapper.Map<GameEventRegistrationDto>(reg);
        }

        [HttpPost]
        public async Task<ActionResult<GameEventRegistrationDto>> Create(GameEventRegistrationCreateDto dto)
        {
            var alreadyRegistered = await _context.EventRegistrations
                .AnyAsync(r => r.EventId == dto.EventId && r.PlayerId == dto.PlayerId);
            if (alreadyRegistered)
                return BadRequest("This player is already registered for this event.");

            var reg = _mapper.Map<GameEventRegistration>(dto);
            reg.DateRegistered = DateTime.UtcNow;

            _context.EventRegistrations.Add(reg);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = reg.Id }, _mapper.Map<GameEventRegistrationDto>(reg));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reg = await _context.EventRegistrations.FindAsync(id);
            if (reg is null) return NotFound();

            _context.EventRegistrations.Remove(reg);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
