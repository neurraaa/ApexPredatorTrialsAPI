using System.Collections.Concurrent;
using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly ConcurrentDictionary<int, T> _store = new();
        private int _nextId = 1;
        private readonly object _idLock = new();
        public IEnumerable<T> GetAll() => _store.Values.ToList();
        public T? GetById(int id) => _store.TryGetValue(id, out var entity) ? entity : default;
        public T Add(T entity)
        {
            lock (_idLock)
            {
                entity.Id = _nextId++;
            }

            _store[entity.Id] = entity;

            return entity;
        }
        public bool Update(T entity)
        {
            if (!_store.ContainsKey(entity.Id)) return false;

            _store[entity.Id] = entity;

            return true;
        }
        public bool Delete(int id) => _store.TryRemove(id, out _);
    }
}
