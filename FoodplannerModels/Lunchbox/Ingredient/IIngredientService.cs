namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the ingredient.
*/
public interface IIngredientService
{
    Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
    Task<Ingredient> GetIngredientByIdAsync(int id);
    Task<int> CreateIngredientAsync(Ingredient ingredient);
    Task<int> UpdateIngredientAsync(Ingredient ingredient, int id);
    Task<int> DeleteIngredientAsync(int id);
}
