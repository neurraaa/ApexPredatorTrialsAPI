using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<bool> UsernameExistsAsync(string username) =>
            await DbSet.AnyAsync(u => u.Username == username);
    }
}
