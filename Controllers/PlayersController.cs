using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PlayersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
        {
            var players = await _context.Players.Include(p => p.PlayerStats).ToListAsync();
            return Ok(_mapper.Map<List<PlayerDto>>(players));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
        {
            var player = await _context.Players
                .Include(p => p.PlayerStats)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (player is null) return NotFound();
            return _mapper.Map<PlayerDto>(player);
        }

        [HttpGet("region/{region}")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayersByRegion(string region)
        {
            var players = await _context.Players.Where(p => p.Region.ToUpper() == region.ToUpper()).ToListAsync();
            return Ok(_mapper.Map<List<PlayerDto>>(players));
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> CreatePlayer(PlayerWriteDto dto)
        {
            var player = _mapper.Map<Player>(dto);
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, _mapper.Map<PlayerDto>(player));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, PlayerWriteDto dto)
        {
            var player = await _context.Players.FindAsync(id);
            if (player is null) return NotFound();

            _mapper.Map(dto, player);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player is null) return NotFound();

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
