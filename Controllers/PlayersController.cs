using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _service;

        public PlayersController(IPlayerService service) => _service = service;

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
        public async Task<ActionResult<PlayerDto>> CreatePlayer(PlayerWriteDto dto)
        {
            var player = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, PlayerWriteDto dto) =>
            await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
