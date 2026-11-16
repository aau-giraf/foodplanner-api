using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

public interface IIngredientsService
{
    Task<IEnumerable<Ingredient>> GetAllIngredientsByUserAsync(int userId);
    Task<Ingredient?> GetIngredientByIdAsync(int id);
    Task<int> CreateIngredientAsync(Ingredient ingredientDTO);
    Task<int> UpdateIngredientAsync(Ingredient ingredientDTO);
    Task<int> DeleteIngredientAsync(int id);
}
