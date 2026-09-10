using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class MatchRepository : Repository<Match>, IMatchRepository
    {
        public MatchRepository(AppDbContext context) : base(context) { }

        public async Task<List<Match>> GetAllWithDetailsAsync() =>
            await DbSet.Include(m => m.HunterPlayer).Include(m => m.HumanPlayer).Include(m => m.Map).ToListAsync();

        public async Task<Match?> GetByIdWithDetailsAsync(int id) =>
            await DbSet.Include(m => m.HunterPlayer).Include(m => m.HumanPlayer).Include(m => m.Map)
                .FirstOrDefaultAsync(m => m.Id == id);
    }
}
