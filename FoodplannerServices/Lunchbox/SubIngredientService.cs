using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;

// Adds the SubIngredientService class to the FoodplannerServices.Lunchbox namespace
namespace FoodplannerServices.Lunchbox;

// Service implementation for SubIngredient business logic
public class SubIngredientService(SubIngredientRepository subIngredientRepository)
{
    // Retrieves all sub-ingredients
    public async Task<IEnumerable<SubIngredientDTO>> GetAllSubIngredientsAsync()
    {
        try
        {
            return await subIngredientRepository.GetAllAsync();
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving all sub-ingredients: {exception.Message}", exception);
        }
    }
    
    // Get all sub-ingredients based on a parent ingredient Id
    public async Task<IEnumerable<SubIngredientDTO>> GetAllSubIngredientsByParentIdAsync(int id)
    {
        try 
        {
            return await subIngredientRepository.GetAllByParentIdAsync(id);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving sub-ingredients for parent ingredient ID {id}: {exception.Message}", exception);
        }
    }

    // Creates a new sub-ingredient
    public async Task<int> CreateSubIngredientAsync(SubIngredientDTO subIngredient)
    {
        try
        {
            return await subIngredientRepository.InsertAsync(subIngredient);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error creating sub-ingredient: {exception.Message}", exception);
        }
    }

    // Updates an existing sub-ingredient
    public async Task<int> UpdateSubIngredientAsync(SubIngredientDTO oldSubIngredient, SubIngredientDTO newSubIngredient)
    {
        try
        {
            return await subIngredientRepository.UpdateAsync(oldSubIngredient, newSubIngredient);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error updating sub-ingredient: {exception.Message}", exception);
        }
    }

    // Deletes a sub-ingredient
    public async Task<int> DeleteSubIngredientAsync(SubIngredientDTO subIngredient)
    {
        try
        {
            return await subIngredientRepository.DeleteAsync(subIngredient);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error deleting sub-ingredient: {exception.Message}", exception);
        }
    }
}