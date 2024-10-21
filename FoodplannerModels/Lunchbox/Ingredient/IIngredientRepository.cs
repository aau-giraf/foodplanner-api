namespace FoodplannerModels.Lunchbox;

/**
* Interface for the ingredient repository.
*/
public interface IIngredientRepository
{
    Task<IEnumerable<Ingredient>> GetAllAsync();
    Task<Ingredient> GetByIdAsync(int id);
    Task<int> InsertAsync(Ingredient entity);
    Task<int> UpdateAsync(Ingredient entity, int id);
    Task<int> DeleteAsync(int id);
}
