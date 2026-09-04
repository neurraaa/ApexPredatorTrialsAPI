using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/matches")]
    public class MatchesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MatchesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetAll()
        {
            var matches = await _context.Matches
                .Include(m => m.HunterPlayer)
                .Include(m => m.HumanPlayer)
                .Include(m => m.Map)
                .ToListAsync();

            return Ok(_mapper.Map<List<MatchDto>>(matches));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetById(int id)
        {
            var match = await _context.Matches
                .Include(m => m.HunterPlayer)
                .Include(m => m.HumanPlayer)
                .Include(m => m.Map)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match is null) return NotFound();
            return _mapper.Map<MatchDto>(match);
        }

        [HttpPost]
        public async Task<ActionResult<MatchDto>> Create(MatchWriteDto dto)
        {
            if (dto.HunterPlayerId == dto.HumanPlayerId)
                return BadRequest("A player cannot play both sides of the same match.");

            var match = _mapper.Map<Match>(dto);
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            await _context.Entry(match).Reference(m => m.HunterPlayer).LoadAsync();
            await _context.Entry(match).Reference(m => m.HumanPlayer).LoadAsync();
            await _context.Entry(match).Reference(m => m.Map).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = match.Id }, _mapper.Map<MatchDto>(match));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MatchWriteDto dto)
        {
            if (dto.HunterPlayerId == dto.HumanPlayerId)
                return BadRequest("A player cannot play both sides of the same match.");

            var match = await _context.Matches.FindAsync(id);
            if (match is null) return NotFound();

            _mapper.Map(dto, match);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match is null) return NotFound();

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
