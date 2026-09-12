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

        public async Task<User?> GetByUsernameAsync(string username) =>
            await DbSet.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<int> CountByRoleAsync(string role) =>
            await DbSet.CountAsync(u => u.Role.ToLower() == role.ToLower());
    }
}
