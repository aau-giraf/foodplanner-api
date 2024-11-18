using FoodplannerModels.Account;

namespace FoodplannerModels.Lunchbox;

/**
* Interface for the ingredient repository.
*/
public interface IIngredientRepository
{
    // Gets all ingredients asynchronously.
    Task<IEnumerable<Ingredient>> GetAllAsync();
    // Gets all ingredients by user asynchronously.
    Task<IEnumerable<Ingredient>> GetAllByUserAsync(int id);
    // Gets an ingredient by ID asynchronously.
    Task<Ingredient> GetByIdAsync(int id);
    // Inserts a new ingredient asynchronously.
    Task<int> InsertAsync(Ingredient entity);
    // Updates an existing ingredient asynchronously.
    Task<int> UpdateAsync(Ingredient entity, int id);
    // Deletes an ingredient by ID asynchronously.
    Task<int> DeleteAsync(int id);
}
