using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class GameEventRepository : Repository<GameEvent>, IGameEventRepository
    {
        public GameEventRepository(AppDbContext context) : base(context) { }
    }
}
