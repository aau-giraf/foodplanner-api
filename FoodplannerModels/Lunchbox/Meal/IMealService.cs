namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the meal.
*/
public interface IMealService
{
    Task<IEnumerable<Meal>> GetAllMealsAsync();
    Task<Meal> GetMealByIdAsync(int id);
    Task<int> CreateMealAsync(Meal meal);
    Task<int> UpdateMealAsync(Meal meal);
    Task<int> DeleteMealAsync(int id);
}
