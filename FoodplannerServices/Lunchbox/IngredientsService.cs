using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;

// Adds the IngredientsService class to the FoodplannerServices.Lunchbox namespace
namespace FoodplannerServices.Lunchbox;

// The service for the Ingredient class.
public class IngredientsService(IngredientsRepository ingredientRepository)
{
    public async Task<IEnumerable<IngredientDTO>> GetAllIngredientsAsync()
    { 
        try
        {
            return await ingredientRepository.GetAllAsync();
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving all ingredients: {exception.Message}", exception);
        }
    }

    // Retrieves all ingredients by user.
    public async Task<IEnumerable<IngredientDTO>> GetAllIngredientsByUserAsync(int userId)
    {
        try
        {
            return await ingredientRepository.GetAllByUserAsync(userId);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving ingredients for user ID {userId}: {exception.Message}", exception);
        }
    }

    // Retrieves a specific ingredient by its ID.
    public async Task<IngredientDTO?> GetIngredientByIdAsync(int id)
    {
        try
        {
            var ingredient = await ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return null;
            }
            return ingredient;
        }
        catch (Exception exception)
        {
            throw new Exception($"Error retrieving ingredient with ID {id}: {exception.Message}", exception);
        }
    }

    // Creates a new ingredient in the repository.
    public async Task<int> CreateIngredientAsync(IngredientDTO ingredientDTO)
    {
        try
        {
            return await ingredientRepository.InsertAsync(ingredientDTO);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error creating ingredient: {exception.Message}", exception);
        }
    }

    // Updates an existing ingredient in the repository by ID.
    public async Task<int> UpdateIngredientAsync(IngredientDTO ingredientDTO)
    {
        try
        {
            return await ingredientRepository.UpdateAsync(ingredientDTO);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error updating ingredient with ID {ingredientDTO.Id}: {exception.Message}", exception);
        }
    }

    // Deletes an ingredient from the repository by ID.
    public async Task<int> DeleteIngredientAsync(int id)
    {
        try
        {
            return await ingredientRepository.DeleteAsync(id);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error deleting ingredient with ID {id}: {exception.Message}", exception);
        }
    }
}