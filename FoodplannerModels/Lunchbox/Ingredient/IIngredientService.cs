using FoodplannerModels.Account;

namespace FoodplannerModels.Lunchbox;

// Temperary interface for the ingredient.
public interface IIngredientService
{
    // Gets all ingredients asynchronously.
    Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
    // Gets all ingredients by userasynchronously.
    Task<List<Ingredient>> GetAllIngredientsByUserAsync(int user);
    // Gets an ingredient by ID asynchronously.
    Task<Ingredient> GetIngredientByIdAsync(int id);
    // Creates a new ingredient asynchronously.
    Task<int> CreateIngredientAsync(Ingredient ingredient);
    // Updates an existing ingredient asynchronously.
    Task<int> UpdateIngredientAsync(Ingredient ingredient, int id);
    // Deletes an ingredient by ID asynchronously.
    Task<int> DeleteIngredientAsync(int id);
}