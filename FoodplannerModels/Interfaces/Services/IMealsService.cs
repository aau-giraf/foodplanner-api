using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

public interface IMealsService
{
    Task<IEnumerable<MealWithIngredientsDTO>> GetAllMealsAsync();

    Task<IEnumerable<MealWithIngredientsDTO>> GetAllMealsByUserAndDateAsync(
        int userId, string date);

    Task<MealWithIngredientsDTO?> GetMealByIdAsync(int id);

    Task<int> CreateMealAsync(MealCreateDTO mealCreateDTO, int userId);

    Task<int> UpdateMealAsync(MealDTO mealDTO);

    Task<int> DeleteMealAsync(int id);

    Task<IEnumerable<MealWithIngredientsDTO>> GetAllTemplatesByUserAsync(
        int userId);

    Task<int> UpdateTemplateStatusAsync(int id, bool template, int userId);

    Task<IEnumerable<IngredientDTO>> GetUniqueIngredientsFromMealsAsync(
        List<int> mealIds, int userId);
}