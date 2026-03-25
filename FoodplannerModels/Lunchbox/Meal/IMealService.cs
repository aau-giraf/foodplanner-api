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
    Task<int> UpdateMealAsync(MealDTO mealDto, int id);
    // Deletes an meal by ID asynchronously.
    Task<int> DeleteMealAsync(int id);
    // get all meals where template is 1
    Task<IEnumerable<MealDTO>> GetAllTemplatesByUserAsync(int userId);
    // Update a meals template status via its ID
    Task<int> UpdateTemplateStatusAsync(int id, bool template, int userId);
    // Get unique ingredients by a list of meal IDs and return ingredients
    Task<IEnumerable<Ingredient>> GetUniqueIngredientsFromMealsAsync(List<int> mealIds, int userId); // Add userId
}
