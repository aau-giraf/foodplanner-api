namespace FoodplannerModels.Lunchbox;

/**
* Interface for the ingredient repository.
*/
public interface IIngredientRepository
{
    Task<IEnumerable<Ingredient>> GetAllAsync();
    Task<Ingredient> GetByNameAsync(string name, string user);
    Task<int> InsertAsync(Ingredient entity);
    Task<int> UpdateAsync(Ingredient entity, string name, string user);
    Task<int> DeleteAsync(string name, string user);
}
