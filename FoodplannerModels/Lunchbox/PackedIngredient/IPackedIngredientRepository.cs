namespace FoodplannerModels.Lunchbox;

/**
* Interface for the meal repository.
*/
public interface IPackedIngredientRepository
{
    Task<IEnumerable<PackedIngredient>> GetAllAsync();
    Task<IEnumerable<PackedIngredient>> GetAllByMealIdAsync(int id);
    Task<PackedIngredient> GetByIdAsync(int id);
    Task<int> InsertAsync(int meal_id, int ingredient_id);
    Task<int> UpdateAsync(PackedIngredient packedIngredient, int id);
    Task<int> DeleteAsync(int id);
    Task<bool> UpdateOrderAsync(int id, int order);
}