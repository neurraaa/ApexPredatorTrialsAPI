using Microsoft.AspNetCore.Mvc;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Controllers
{
    [Route("api/players")]
    public class PlayerController : RepositoryControllerBase<Player>
    {
        public PlayerController(IRepository<Player> repository) : base(repository) { }

        [HttpGet("region/{region}")]
        public ActionResult<IEnumerable<Player>> GetByRegion(string region) => Ok(Repository.GetAll().Where(p => p.Region.Equals(region,StringComparison.OrdinalIgnoreCase)));
    }
}
