using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class MatchResultsRepository : Repository<MatchResults>, IMatchResultsRepository
    {
        public MatchResultsRepository(AppDbContext context) : base(context) { }

        public async Task<List<MatchResults>> GetAllWithPlayersAsync() =>
            await DbSet.Include(r => r.Winner).Include(r => r.Loser).ToListAsync();

        public async Task<MatchResults?> GetByIdWithPlayersAsync(int id) =>
            await DbSet.Include(r => r.Winner).Include(r => r.Loser).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<MatchResults?> GetByMatchIdAsync(int matchId) =>
            await DbSet.Include(r => r.Winner).Include(r => r.Loser).FirstOrDefaultAsync(r => r.MatchId == matchId);
    }
}
