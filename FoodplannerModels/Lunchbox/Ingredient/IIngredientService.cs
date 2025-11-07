using FoodplannerModels.Account;

namespace FoodplannerModels.Lunchbox;

// Temperary interface for the ingredient.
public interface IIngredientService
{
    // Gets all ingredients asynchronously.
    Task<IEnumerable<IngredientDTO>> GetAllIngredientsAsync();
    // Gets all ingredients by userasynchronously.
    Task<IEnumerable<IngredientDTO>> GetAllIngredientsByUserAsync(int user);
    // Gets an ingredient by ID asynchronously.
    Task<IngredientDTO> GetIngredientByIdAsync(int id);
    // Creates a new ingredient asynchronously.
    Task<int> CreateIngredientAsync(IngredientDTO ingredient, int id);
    // Updates an existing ingredient asynchronously.
    Task<int> UpdateIngredientAsync(IngredientDTO ingredient, int id);
    // Deletes an ingredient by ID asynchronously.
    Task<int> DeleteIngredientAsync(int id);
}