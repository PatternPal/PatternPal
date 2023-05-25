#nullable enable
namespace PatternPal.LoggingServer.Data.Interfaces
{
    /// <summary>
    /// Repository interface for database access. All entities must implement this interface, but they can add their own methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {

        Task<List<T>> GetAll();
        Task<T?> GetById(string id);
        Task<T> Insert(T entity);
        Task<T> Update(T entity);
        Task Delete(string id);

    }
}
