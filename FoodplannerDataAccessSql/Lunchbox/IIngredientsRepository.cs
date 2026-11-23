using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Lunchbox;

public interface IIngredientsRepository
{
    Task<IEnumerable<IngredientDTO>> GetAllAsync();

    Task<IEnumerable<IngredientDTO>> GetAllByUserAsync(int id);

    Task<IngredientDTO?> GetByIdAsync(int id);

    Task<int> InsertAsync(IngredientDTO entity, IEnumerable<int> categoryIds);

    Task<int> UpdateAsync(IngredientDTO entity);

    Task<int> DeleteAsync(int id);
}
