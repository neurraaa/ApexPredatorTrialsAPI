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
        private readonly ILogger<PlayerStatisticsController> _logger;

        public PlayerStatisticsController(IPlayerStatsService service, ILogger<PlayerStatisticsController> logger)
        {
            _service = service;
            _logger = logger;
        }

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

            _logger.LogInformation("Statistics ({StatsId}) created", stats.Id);

            return CreatedAtAction(nameof(GetById), new { id = stats.Id }, stats);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PlayerStatsWriteDto dto)
        {
            if (!await _service.UpdateAsync(id, dto))
            {
                _logger.LogWarning("Update failed: Statistics ({StatsId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Statistics ({StatsId}) updated", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.DeleteAsync(id))
            {
                _logger.LogWarning("Delete failed: Statistics ({StatsId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Statistics ({StatsId}) deleted", id);

            return NoContent();
        }
    }
}
