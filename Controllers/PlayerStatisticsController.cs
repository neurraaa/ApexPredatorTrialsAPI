using Microsoft.AspNetCore.Mvc;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/player-statistics")]
    public class PlayerStatisticsController : ControllerBase
    {
        private readonly IPlayerStatsService _service;

        public PlayerStatisticsController(IPlayerStatsService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerStatsDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerStatsDto>> GetById(int id)
        {
            var stats = await _service.GetByIdAsync(id);
            return stats is null ? NotFound() : Ok(stats);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerStatsDto>> Create(PlayerStatsWriteDto dto)
        {
            var stats = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = stats.Id }, stats);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PlayerStatsWriteDto dto) =>
            await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
