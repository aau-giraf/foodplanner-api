using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;

// Adds the PackedIngredientService class to the FoodplannerServices.Lunchbox namespace
namespace FoodplannerServices.Lunchbox;

// Service for the Packed class.
public class PackedService(IPackedRepository packedRepository) : IPackedService
{
    // Retrieves all packed ingredients
    public async Task<IEnumerable<PackedDTO>> GetAllPackedAsync()
    {
        try
        {
            return await packedRepository.GetAllAsync();
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving all packed ingredients: {exception.Message}", exception);
        }
    }
    
    // Get all packed ingredients based on a meal ID
    public async Task<IEnumerable<PackedDTO>> GetAllPackedByMealIdAsync(int id)
    {
        try
        {
            return await packedRepository.GetAllByMealIdAsync(id);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving packed ingredients for meal ID {id}: {exception.Message}", exception);
        }
    }

    // Creates a new packed ingredient
    public async Task<int> CreatePackedAsync(PackedDTO packed)
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
    public async Task<int> UpdatePackedAsync(PackedDTO oldPacked, PackedDTO newPacked)
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
    public async Task<int> DeletePackedAsync(PackedDTO packed)
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