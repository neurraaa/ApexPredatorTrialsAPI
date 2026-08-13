using Microsoft.AspNetCore.Mvc;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Controllers
{
    [Route("api/matches")]
    public class MatchController : RepositoryControllerBase<Match>
    {
        public MatchController(IRepository<Match> repository) : base(repository) { }
        public override ActionResult<Match> Create(Match match)
        {
            if (match.HunterPlayerId == match.HumanPlayerId)
            {
                return BadRequest("A player cannot play both sides of the same match.");
            }

            return base.Create(match);
        }
    }
}
