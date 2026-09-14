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
            await DbSet.Where(s => s.EventId == eventId)
                .Include(s => s.HunterPlayer)
                .Include(s => s.HumanPlayer)
                .Include(s => s.WinnerPlayer)
                .ToListAsync();

        public async Task<GameEventSchedule?> GetByIdWithMatchAsync(int id) =>
            await DbSet.Include(s => s.Match)
                .Include(s => s.HunterPlayer)
                .Include(s => s.HumanPlayer)
                .Include(s => s.WinnerPlayer)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<List<GameEventSchedule>> GetByIdsAsync(IEnumerable<int> ids) =>
            await DbSet.Where(s => ids.Contains(s.Id))
                .Include(s => s.HunterPlayer)
                .Include(s => s.HumanPlayer)
                .ToListAsync();

        public async Task<List<GameEventSchedule>> GetByEventIdAndRoundAsync(int eventId, string round) =>
            await DbSet.Where(s => s.EventId == eventId && s.Round == round)
                .Include(s => s.HunterPlayer)
                .Include(s => s.HumanPlayer)
                .ToListAsync();
    }
}
