namespace FoodplannerModels.Lunchbox;

/**
* Interface for the meal repository.
*/
public interface IPackedIngredientRepository
{
    Task<IEnumerable<PackedIngredient>> GetAllAsync();
    Task<IEnumerable<PackedIngredient>> GetAllByMealIdAsync(int id);
    Task<PackedIngredient> GetByIdAsync(int id);
    Task<int> InsertAsync(int meal_ref, int ingredient_ref);
    Task<int> UpdateAsync(PackedIngredient packedIngredient, int id);
    Task<int> DeleteAsync(int id);
}