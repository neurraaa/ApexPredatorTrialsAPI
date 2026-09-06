using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IGameEventScheduleRepository : IRepository<GameEventSchedule>
    {
        Task<List<GameEventSchedule>> GetByEventIdAsync(int eventId);
        Task<GameEventSchedule?> GetByIdWithMatchAsync(int id);
    }
}
