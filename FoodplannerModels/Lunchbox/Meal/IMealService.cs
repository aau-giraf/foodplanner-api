namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the meal.
*/
public interface IMealService
{
    // Gets all meals asynchronously.
    Task<IEnumerable<MealDTO>> GetAllMealsAsync();
    // Gets all meals by user id asynchronously.
    Task<IEnumerable<MealDTO>> GetAllMealsByUserAsync(int user_id, string date);
    // Gets an meal by ID asynchronously.
    Task<MealDTO?> GetMealByIdAsync(int id);
    // Creates a new meal asynchronously.
    Task<int> CreateMealAsync(MealCreateDTO meal, int id);
    // Updates an existing meal asynchronously.
    Task<int> UpdateMealAsync(Meal meal, int id);
    // Deletes an meal by ID asynchronously.
    Task<int> DeleteMealAsync(int id);
}
