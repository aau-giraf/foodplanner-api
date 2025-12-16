namespace FoodplannerModels.Lunchbox;

/**
* Interface for the meal repository.
*/
public interface IPackedIngredientRepository : IGenericRepository<PackedIngredient>
{
    Task<IEnumerable<PackedIngredient>> GetAllByMealIdAsync(int id);
    Task<bool> UpdateOrderAsync(int id, int order);
}