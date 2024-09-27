using foodplanner_api.Models;

namespace foodplanner_api.Data.Repositories;

public interface IRepository<T> where T : class{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<int> InsertAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(int id);
}