using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/players")]
    [Authorize]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _service;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(IPlayerService service, ILogger<PlayersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int id)
        {
            var player = await _service.GetByIdAsync(id);
            return player is null ? NotFound() : Ok(player);
        }

        [HttpGet("region/{region}")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayersByRegion(string region) =>
            Ok(await _service.GetByRegionAsync(region));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PlayerDto>> CreatePlayer(PlayerWriteDto dto)
        {
            var player = await _service.CreateAsync(dto);

            _logger.LogInformation("Player ({PlayerId}) created ({Name})", player.Id, player.Name);

            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePlayer(int id, PlayerWriteDto dto)
        {
            if (!await _service.UpdateAsync(id, dto))
            {
                _logger.LogWarning("Update failed: Player ({PlayerId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Player ({PlayerId}) updated", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            if (!await _service.DeleteAsync(id))
            {
                _logger.LogWarning("Delete failed: Player ({PlayerId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Player ({PlayerId}) deleted", id);

            return NoContent();
        }
    }
}
