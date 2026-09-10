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
        private readonly ILogger<MatchesController> _logger;

        public MatchesController(IMatchService service, ILogger<MatchesController> logger)
        {
            _service = service;
            _logger = logger;
        }

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

            if (result.Status == ServiceStatus.Invalid)
            {
                _logger.LogWarning("Failed to create match: {Error}", result.Error);

                return BadRequest(result.Error);
            }

            _logger.LogInformation("Created match {MatchId}", result.Data!.Id);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MatchWriteDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            switch (result.Status)
            {
                case ServiceStatus.Invalid:
                    _logger.LogWarning("Failed to update match {MatchId}: {Error}", id, result.Error);

                    return BadRequest(result.Error);
                case ServiceStatus.NotFound:
                    _logger.LogWarning("Update failed, match {MatchId} not found", id);

                    return NotFound();
                default:
                    _logger.LogInformation("Updated match {MatchId}", id);

                    return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.DeleteAsync(id))
            {
                _logger.LogWarning("Delete failed: Match ({MatchId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Match ({MatchId}) deleted", id);

            return NoContent();
        }
    }
}
