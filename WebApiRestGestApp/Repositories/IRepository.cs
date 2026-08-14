using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiRestGestApp.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        // params object[] permite enviar 1 llave (Guid/int) o varias (llaves compuestas)
        Task<T?> GetByIdAsync(params object[] keyValues);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(params object[] keyValues);
    }
}