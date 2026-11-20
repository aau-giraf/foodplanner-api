using FoodplannerModels;
using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

// IGenericRepository contains generic CRUD operations
public interface ICategoryRepository : IGenericRepository<CategoryDTO>
{
    // Method for checking if category already exists
    Task<bool> CategoryExistsAsync(string name);

}