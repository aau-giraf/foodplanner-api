namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the ingredient.
*/
public interface IIngredientService
{
    Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
    Task<Ingredient> GetIngredientByNameAsync(string name);
    Task<int> CreateIngredientAsync(Ingredient ingredient);
}
