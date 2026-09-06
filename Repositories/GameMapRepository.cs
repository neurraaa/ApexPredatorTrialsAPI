using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class GameMapRepository : Repository<GameMap>, IGameMapRepository
    {
        public GameMapRepository(AppDbContext context) : base(context) { }
    }
}
