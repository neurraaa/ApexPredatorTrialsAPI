using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class PlayerStatsRepository : Repository<PlayerStats>, IPlayerStatsRepository
    {
        public PlayerStatsRepository(AppDbContext context) : base(context) { }
    }
}
