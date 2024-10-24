namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the meal.
*/
public interface IMealService
{
    // Gets all meal asynchronously.
    Task<IEnumerable<Meal>> GetAllMealsAsync();
    // Gets an meal by ID asynchronously.
    Task<Meal> GetMealByIdAsync(int id);
    // Creates a new meal asynchronously.
    Task<int> CreateMealAsync(Meal meal);
    // Updates an existing meal asynchronously.
    Task<int> UpdateMealAsync(Meal meal, int id);
    // Deletes an meal by ID asynchronously.
    Task<int> DeleteMealAsync(int id);
}
