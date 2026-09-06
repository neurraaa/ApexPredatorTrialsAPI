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

        public MatchResultsController(IMatchResultsService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchResultsDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchResultsDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MatchResultsDto>> Create(MatchResultsCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return result.Status switch
            {
                ServiceStatus.NotFound => BadRequest("The referenced Match does not exist."),
                ServiceStatus.Invalid => BadRequest(result.Error),
                _ => CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
