using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IGameEventRegistrationRepository : IRepository<GameEventRegistration>
    {
        Task<bool> IsPlayerRegisteredAsync(int eventId, int playerId);
    }
}
