using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/match-results")]
    public class MatchResultsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MatchResultsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchResultsDto>>> GetAll()
        {
            var results = await _context.MatchResults
                .Include(r => r.Winner)
                .Include(r => r.Loser)
                .ToListAsync();

            return Ok(_mapper.Map<List<MatchResultsDto>>(results));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchResultsDto>> GetById(int id)
        {
            var result = await _context.MatchResults
                .Include(r => r.Winner)
                .Include(r => r.Loser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result is null) return NotFound();
            return _mapper.Map<MatchResultsDto>(result);
        }

        [HttpPost]
        public async Task<ActionResult<MatchResultsDto>> Create(MatchResultsCreateDto dto)
        {
            var match = await _context.Matches.FindAsync(dto.MatchId);
            if (match is null) return BadRequest("The referenced Match does not exist.");

            var validPlayerIds = new[] { match.HunterPlayerId, match.HumanPlayerId };
            if (!validPlayerIds.Contains(dto.WinnerId) || !validPlayerIds.Contains(dto.LoserId))
                return BadRequest("Winner and Loser must be the two players from the referenced Match.");
            if (dto.WinnerId == dto.LoserId)
                return BadRequest("Winner and Loser cannot be the same player.");

            var result = _mapper.Map<MatchResults>(dto);
            result.DateTimeConcluded = DateTime.UtcNow;

            _context.MatchResults.Add(result);
            await _context.SaveChangesAsync();

            await _context.Entry(result).Reference(r => r.Winner).LoadAsync();
            await _context.Entry(result).Reference(r => r.Loser).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, _mapper.Map<MatchResultsDto>(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _context.MatchResults.FindAsync(id);
            if (result is null) return NotFound();

            _context.MatchResults.Remove(result);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
