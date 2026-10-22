using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

public interface IIngredientsService
{
    Task<IEnumerable<IngredientDTO>> GetAllIngredientsAsync();
    Task<IEnumerable<IngredientDTO>> GetAllIngredientsByUserAsync(int userId);
    Task<IngredientDTO?> GetIngredientByIdAsync(int id);
    Task<int> CreateIngredientAsync(IngredientDTO ingredientDTO);
    Task<int> UpdateIngredientAsync(IngredientDTO ingredientDTO);
    Task<int> DeleteIngredientAsync(int id);
}
