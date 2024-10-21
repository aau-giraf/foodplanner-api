namespace FoodplannerModels.Lunchbox;

/**
* Temperary interface for the packedIngredient.
*/
public interface IPackedIngredientService
{
    Task<IEnumerable<PackedIngredient>> GetAllPackedIngredientsAsync();
    Task<PackedIngredient> GetPackedIngredientByIdAsync(int Id);
    Task<int> CreatePackedIngredientAsync(PackedIngredient packedIngredient);
}