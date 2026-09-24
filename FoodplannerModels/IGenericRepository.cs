namespace FoodplannerModels;

// Generic repository interface for CRUD operations.
public interface IGenericRepository<T> where T : class{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<int> InsertAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(int id);
}