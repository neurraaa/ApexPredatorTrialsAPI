using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class GameEventScheduleRepository : Repository<GameEventSchedule>, IGameEventScheduleRepository
    {
        public GameEventScheduleRepository(AppDbContext context) : base(context) { }

        private IQueryable<GameEventSchedule> WithMatchDetails() =>
            DbSet.Include(s => s.Match).ThenInclude(m => m!.HunterPlayer)
                .Include(s => s.Match).ThenInclude(m => m!.HumanPlayer)
                .Include(s => s.Match).ThenInclude(m => m!.Map)
                .Include(s => s.Match).ThenInclude(m => m!.MatchResults);

        public async Task<List<GameEventSchedule>> GetByEventIdAsync(int eventId) =>
            await WithMatchDetails().Where(s => s.EventId == eventId).ToListAsync();

        public async Task<GameEventSchedule?> GetByIdWithMatchAsync(int id) =>
            await WithMatchDetails().FirstOrDefaultAsync(s => s.Id == id);

        public async Task<List<GameEventSchedule>> GetByIdsAsync(IEnumerable<int> ids) =>
            await WithMatchDetails().Where(s => ids.Contains(s.Id)).ToListAsync();

        public async Task<List<GameEventSchedule>> GetByEventIdAndRoundAsync(int eventId, string round) =>
            await WithMatchDetails().Where(s => s.EventId == eventId && s.Round == round).ToListAsync();
    }
}
