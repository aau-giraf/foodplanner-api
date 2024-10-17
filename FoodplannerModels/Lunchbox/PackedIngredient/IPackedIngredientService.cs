namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the packedIngredient.
*/
public interface IPackedIngredientService
{
    Task<IEnumerable<PackedIngredient>> GetAllIngredientsAsync();
    Task<PackedIngredient> GetPackedIngredientByIdAsync(int Id);
    Task<int> CreateIngredientAsync(PackedIngredient packedIngredient);
}
