using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

public interface IIngredientsService
{
    Task<IEnumerable<Ingredient>> GetAllIngredientsByUserAsync(int userId);
    Task<Ingredient?> GetIngredientByIdAsync(int id);
    Task<int> CreateIngredientAsync(Ingredient Ingredient,IEnumerable<int> categoryIds);
    Task<int> UpdateIngredientAsync(Ingredient Ingredient);
    Task<int> DeleteIngredientAsync(int id);
}
