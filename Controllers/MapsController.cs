using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MapsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameMapDto>>> GetAll()
        {
            var maps = await _context.Maps.ToListAsync();
            return Ok(_mapper.Map<List<GameMapDto>>(maps));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameMapDto>> GetById(int id)
        {
            var map = await _context.Maps.FindAsync(id);
            if (map is null) return NotFound();
            return _mapper.Map<GameMapDto>(map);
        }

        [HttpPost]
        public async Task<ActionResult<GameMapDto>> Create(GameMapWriteDto dto)
        {
            var map = _mapper.Map<GameMap>(dto);
            _context.Maps.Add(map);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = map.Id }, _mapper.Map<GameMapDto>(map));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GameMapWriteDto dto)
        {
            var map = await _context.Maps.FindAsync(id);
            if (map is null) return NotFound();

            _mapper.Map(dto, map);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var map = await _context.Maps.FindAsync(id);
            if (map is null) return NotFound();

            _context.Maps.Remove(map);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
