namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the packedIngredient.
*/
public interface IPackedIngredientService
{
    Task<IEnumerable<PackedIngredient>> GetAllPackedIngredientsAsync();
    Task<PackedIngredient> GetPackedIngredientByIdAsync(int id);
    Task<int> CreatePackedIngredientAsync(PackedIngredient packedIngredient);
    Task<int> UpdatePackedIngredientAsync(PackedIngredient packedIngredient, int id);
    Task<int> DeletePackedIngredientAsync(int id);
}