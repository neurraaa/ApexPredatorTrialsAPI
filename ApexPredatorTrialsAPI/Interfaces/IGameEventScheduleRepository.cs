using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IGameEventScheduleRepository : IRepository<GameEventSchedule>
    {
        Task<List<GameEventSchedule>> GetByEventIdAsync(int eventId);
        Task<GameEventSchedule?> GetByIdWithMatchAsync(int id);
        Task<List<GameEventSchedule>> GetByIdsAsync(IEnumerable<int> ids);
        Task<List<GameEventSchedule>> GetByEventIdAndRoundAsync(int eventId, string round);
    }
}
