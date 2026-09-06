using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IMatchRepository : IRepository<Match>
    {
        Task<List<Match>> GetAllWithDetailsAsync();
        Task<Match?> GetByIdWithDetailsAsync(int id);
    }
}
