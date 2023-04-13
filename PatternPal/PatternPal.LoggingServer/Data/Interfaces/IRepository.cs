namespace PatternPal.LoggingServer.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T?> GetById(string id);
        Task<T> Insert(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(string id);

    }
}
