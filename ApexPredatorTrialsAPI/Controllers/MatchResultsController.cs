using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApexPredatorTrialsAPI.Controllers
{
    [ApiController]
    [Route("api/match-results")]
    public class MatchResultsController : ControllerBase
    {
        private readonly IMatchResultsService _service;
        private readonly ILogger<MatchResultsController> _logger;

        public MatchResultsController(IMatchResultsService service, ILogger<MatchResultsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchResultsDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchResultsDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("by-match/{matchId}")]
        public async Task<ActionResult<MatchResultsDto>> GetByMatch(int matchId)
        {
            var result = await _service.GetByMatchIdAsync(matchId);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MatchResultsDto>> Create(MatchResultsCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);

            switch (result.Status)
            {
                case ServiceStatus.NotFound:
                    _logger.LogWarning("Failed to create match result: referenced match {MatchId} does not exist", dto.MatchId);

                    return BadRequest("The referenced Match does not exist.");
                case ServiceStatus.Invalid:
                    _logger.LogWarning("Failed to create match result: {Error}", result.Error);

                    return BadRequest(result.Error);
                default:
                    _logger.LogInformation("Created match result {MatchResultId}", result.Data!.Id);

                    return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MatchResultsCreateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            switch (result.Status)
            {
                case ServiceStatus.NotFound:
                    _logger.LogWarning("Update failed: Match Result ({MatchResultId}) not found", id);

                    return NotFound();
                case ServiceStatus.Invalid:
                    _logger.LogWarning("Failed to update match result {MatchResultId}: {Error}", id, result.Error);

                    return BadRequest(result.Error);
                default:
                    _logger.LogInformation("Match Result ({MatchResultId}) updated", id);

                    return Ok(result.Data);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.DeleteAsync(id))
            {
                _logger.LogWarning("Delete failed: Match Results ({MatchResultId}) not found", id);

                return NotFound();
            }

            _logger.LogInformation("Match Results ({MatchResultId}) deleted", id);

            return NoContent();
        }
    }
}
