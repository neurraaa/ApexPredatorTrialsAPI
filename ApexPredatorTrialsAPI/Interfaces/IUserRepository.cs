using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> UsernameExistsAsync(string username);
        Task<User?> GetByUsernameAsync(string username);
        Task<int> CountByRoleAsync(string role);
    }
}
