using FoodplannerModels;
using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

public interface ICategoryRepository : IGenericRepository<CategoryDTO>
{
    //Task<IEnumerable<CategoryDTO>> GetAllAsync();

    // Method for checking if category already exists
    Task<bool> CategoryExistsAsync(string name);


}