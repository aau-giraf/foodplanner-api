namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the ingredient.
*/
public interface IIngredientService
{
    Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
    Task<Ingredient> GetIngredientByNameAsync(string name, string user);
    Task<int> CreateIngredientAsync(Ingredient ingredient);
    Task<int> UpdateIngredientAsync(Ingredient ingredient);
    Task<int> DeleteIngredientAsync(string name, string user);
}
