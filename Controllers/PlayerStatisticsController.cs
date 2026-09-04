using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/player-statistics")]
    public class PlayerStatisticsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PlayerStatisticsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerStatsDto>>> GetAll()
        {
            var stats = await _context.PlayersStatistics.ToListAsync();
            return Ok(_mapper.Map<List<PlayerStatsDto>>(stats));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerStatsDto>> GetById(int id)
        {
            var stats = await _context.PlayersStatistics.FindAsync(id);
            if (stats is null) return NotFound();
            return _mapper.Map<PlayerStatsDto>(stats);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerStatsDto>> Create(PlayerStatsWriteDto dto)
        {
            var stats = _mapper.Map<PlayerStats>(dto);
            _context.PlayersStatistics.Add(stats);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = stats.Id }, _mapper.Map<PlayerStatsDto>(stats));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PlayerStatsWriteDto dto)
        {
            var stats = await _context.PlayersStatistics.FindAsync(id);
            if (stats is null) return NotFound();

            _mapper.Map(dto, stats);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var stats = await _context.PlayersStatistics.FindAsync(id);
            if (stats is null) return NotFound();

            _context.PlayersStatistics.Remove(stats);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
