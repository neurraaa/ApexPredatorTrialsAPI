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

        public MapsController(IGameMapService service) => _service = service;

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
            return CreatedAtAction(nameof(GetById), new { id = map.Id }, map);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GameMapWriteDto dto) =>
            await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
