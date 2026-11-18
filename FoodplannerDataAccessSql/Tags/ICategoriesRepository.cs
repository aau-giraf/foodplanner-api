using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

public interface ICategoryRepository
{
Task<IEnumerable<CategoryDTO>> GetAllAsync();

Task<CategoryDTO> InsertAsync(CategoryDTO category);


}