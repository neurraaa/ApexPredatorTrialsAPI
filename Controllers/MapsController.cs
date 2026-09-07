using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private readonly IGameMapService _service;
        private readonly ILogger<MapsController> _logger;

        public MapsController(IGameMapService service, ILogger<MapsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameMapDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<GameMapDto>> GetById(int id)
        {
            var map = await _service.GetByIdAsync(id);
            return map is null ? NotFound() : Ok(map);
        }

        [HttpPost]
        public async Task<ActionResult<GameMapDto>> Create(GameMapWriteDto dto)
        {
            var map = await _service.CreateAsync(dto);

            _logger.LogInformation("Map ({MapId}) created ({Name})", map.Id, map.Name);

            return CreatedAtAction(nameof(GetById), new { id = map.Id }, map);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GameMapWriteDto dto)
        {
            if (!await _service.UpdateAsync(id, dto))
            {
                _logger.LogWarning("Update failed: Map ({MapId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Map ({MapId}) updated", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.DeleteAsync(id))
            {
                _logger.LogWarning("Delete failed: Map ({MapId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Map ({MapId}) deleted", id);

            return NoContent();
        }
    }
}
