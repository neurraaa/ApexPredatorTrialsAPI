using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IMatchResultsRepository : IRepository<MatchResults>
    {
        Task<List<MatchResults>> GetAllWithPlayersAsync();
        Task<MatchResults?> GetByIdWithPlayersAsync(int id);
    }
}
