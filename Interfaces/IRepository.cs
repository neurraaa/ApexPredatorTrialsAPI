namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        T Add(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}
