using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class GameEventRegistrationRepository : Repository<GameEventRegistration>, IGameEventRegistrationRepository
    {
        public GameEventRegistrationRepository(AppDbContext context) : base(context) { }

        public async Task<bool> IsPlayerRegisteredAsync(int eventId, int playerId) =>
            await DbSet.AnyAsync(r => r.EventId == eventId && r.PlayerId == playerId);
    }
}
