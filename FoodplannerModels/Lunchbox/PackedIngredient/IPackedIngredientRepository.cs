namespace FoodplannerModels.Lunchbox;

/**
* Interface for the meal repository.
*/
public interface IPackedIngredientRepository
{
    Task<IEnumerable<PackedIngredient>> GetAllAsync();
    Task<PackedIngredient> GetByIdAsync(int id);
    Task<int> InsertAsync(PackedIngredient packedIngredient);
    Task<int> UpdateAsync(PackedIngredient packedIngredient);
    Task<int> DeleteAsync(int id);
}