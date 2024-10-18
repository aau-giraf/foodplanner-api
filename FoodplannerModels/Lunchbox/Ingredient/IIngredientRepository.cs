namespace FoodplannerModels.Lunchbox;

/**
* Interface for the ingredient repository.
*/
public interface IIngredientRepository
{
    Task<IEnumerable<Ingredient>> GetAllAsync();
    Task<Ingredient> GetByNameAsync(string name);
    Task<int> InsertAsync(Ingredient entity);
}
