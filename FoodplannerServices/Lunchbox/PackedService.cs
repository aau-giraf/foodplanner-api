using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;

// Adds the PackedIngredientService class to the FoodplannerServices.Lunchbox namespace
namespace FoodplannerServices.Lunchbox;

// Service for the Packed class.
public class PackedService(IPackedRepository packedRepository) : IPackedService
{   
    // Creates a new packed ingredient
    public async Task<int> CreatePackedAsync(Packed packed)
    {
        try
        {
            return await packedRepository.InsertAsync(packed);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error creating packed ingredient: {exception.Message}", exception);
        }
    }

    // Updates an existing packed ingredient
    public async Task<int> UpdatePackedAsync(Packed oldPacked, Packed newPacked)
    {
        try
        {
            return await packedRepository.UpdateAsync(oldPacked, newPacked);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error updating packed ingredient: {exception.Message}", exception);
        }
    }

    // Deletes a packed ingredient based on the ID
    public async Task<int> DeletePackedAsync(Packed packed)
    {
        try
        {
            return await packedRepository.DeleteAsync(packed);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error deleting packed ingredient: {exception.Message}", exception);
        }
    }
}