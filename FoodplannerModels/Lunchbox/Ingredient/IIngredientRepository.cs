using FoodplannerModels.Account;

namespace FoodplannerModels.Lunchbox;

/**
* Interface for the ingredient repository.
*/
public interface IIngredientRepository : IGenericRepository<Ingredient>
{
    // Gets all ingredients by user asynchronously.
    Task<IEnumerable<Ingredient>> GetAllByUserAsync(int id);
}
