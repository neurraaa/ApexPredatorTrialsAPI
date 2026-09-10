using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class GameEventScheduleRepository : Repository<GameEventSchedule>, IGameEventScheduleRepository
    {
        public GameEventScheduleRepository(AppDbContext context) : base(context) { }

        public async Task<List<GameEventSchedule>> GetByEventIdAsync(int eventId) =>
            await DbSet.Where(s => s.EventId == eventId).Include(s => s.Match).ToListAsync();

        public async Task<GameEventSchedule?> GetByIdWithMatchAsync(int id) =>
            await DbSet.Include(s => s.Match).FirstOrDefaultAsync(s => s.Id == id);
    }
}
