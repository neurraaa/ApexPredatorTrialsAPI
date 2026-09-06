using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/matches")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _service;

        public MatchesController(IMatchService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetById(int id)
        {
            var match = await _service.GetByIdAsync(id);
            return match is null ? NotFound() : Ok(match);
        }

        [HttpPost]
        public async Task<ActionResult<MatchDto>> Create(MatchWriteDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result.Status == ServiceStatus.Invalid) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MatchWriteDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result.Status switch
            {
                ServiceStatus.Invalid => BadRequest(result.Error),
                ServiceStatus.NotFound => NotFound(),
                _ => NoContent()
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
