using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual async Task<List<T>> GetAllAsync() => await DbSet.ToListAsync();

        public virtual async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

        public void Update(T entity) => Context.Entry(entity).State = EntityState.Modified;

        public void Delete(T entity) => DbSet.Remove(entity);

        public async Task<bool> SaveChangesAsync() => await Context.SaveChangesAsync() >= 0;
    }
}
