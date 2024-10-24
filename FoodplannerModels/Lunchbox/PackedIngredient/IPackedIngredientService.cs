namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the packedIngredient.
*/
public interface IPackedIngredientService
{
    // Retrieves all packed ingredients asynchronously,
    Task<IEnumerable<PackedIngredient>> GetAllPackedIngredientsAsync();
    // Retrieves a specific packed ingredient by its ID asynchronously.
    Task<PackedIngredient> GetPackedIngredientByIdAsync(int id);
    // Creates a new packed ingredient asynchronously.
    Task<int> CreatePackedIngredientAsync(PackedIngredient packedIngredient);
    // Updates an existing packed ingredient by ID asynchronously.
    Task<int> UpdatePackedIngredientAsync(PackedIngredient packedIngredient, int id);
    // Deletes a packed ingredient by its ID asynchronously.
    Task<int> DeletePackedIngredientAsync(int id);
}