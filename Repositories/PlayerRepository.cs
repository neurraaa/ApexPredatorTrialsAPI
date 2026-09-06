using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(AppDbContext context) : base(context) { }

        public async Task<List<Player>> GetAllWithStatsAsync() =>
            await DbSet.Include(p => p.PlayerStats).ToListAsync();

        public async Task<Player?> GetByIdWithStatsAsync(int id) =>
            await DbSet.Include(p => p.PlayerStats).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<List<Player>> GetByRegionAsync(string region) =>
            await DbSet.Where(p => p.Region.ToUpper() == region.ToUpper()).ToListAsync();
    }
}
