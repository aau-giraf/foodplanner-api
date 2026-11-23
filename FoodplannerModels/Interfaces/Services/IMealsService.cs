using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

public interface IMealsService
{
    Task<IEnumerable<MealWithIngredientsDTO>> GetAllMealsByUserAndDateAsync(int userId, string startDate, string endDate);
    Task<MealWithIngredientsDTO?> GetMealByIdAsync(int id);
    Task<int> CreateMealAsync(MealCreateDTO mealCreateDTO, int userId);
    Task<int> UpdateMealAsync(Meal mealDTO);
    Task<int> DeleteMealAsync(int id);
    Task<IEnumerable<MealWithIngredientsDTO>> GetAllTemplatesByUserAsync(int userId);
    Task<int> UpdateTemplateStatusAsync(int id, bool template, int userId);
    Task<IEnumerable<Ingredient>> GetUniqueIngredientsFromMealsAsync(List<int> mealIds, int userId);
}