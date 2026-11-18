using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

public interface ICategoriesRepository
{
Task<IEnumerable<CategoryDTO>> GetAllAsync();

Task<CategoryDTO> InsertAsync(CategoryDTO categories);


}