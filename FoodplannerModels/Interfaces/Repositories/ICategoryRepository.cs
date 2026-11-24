using FoodplannerModels;
using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

// IGenericRepository contains generic CRUD operations
public interface ICategoryRepository
{
    Task<int> InsertAsync(Category entity);
    // Method for checking if category already exists
    Task<bool> CategoryExistsAsync(string name);

}