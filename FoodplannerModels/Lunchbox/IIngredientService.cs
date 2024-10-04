namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the ingredient.
*/
public interface IIngredientService
{
    Task<IEnumerable<Ingredient>> GetAllMealsAsync();
    Task<int> CreateMealAsync(Ingredient ingredient);
    Task<int> UpdateMealAsync(Ingredient ingredient);
    Task<int> DeleteMealAsync(string name);
}
