using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<List<Player>> GetAllWithStatsAsync();
        Task<Player?> GetByIdWithStatsAsync(int id);
        Task<List<Player>> GetByRegionAsync(string region);
    }
}
