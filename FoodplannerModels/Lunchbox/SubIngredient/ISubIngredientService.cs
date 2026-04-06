namespace FoodplannerModels.Lunchbox;

/// <summary>
/// Service interface for SubIngredient business logic
/// </summary>
public interface ISubIngredientService
{
    /// <summary>
    /// Gets all subingredients (for admin purposes)
    /// </summary>
    Task<IEnumerable<SubIngredient>> GetAllSubIngredientsAsync();
    
    /// <summary>
    /// Gets all subingredients for a specific user
    /// </summary>
    Task<IEnumerable<SubIngredient>> GetAllSubIngredientsByUserAsync(int userId);
    
    /// <summary>
    /// Gets a specific subingredient by ID
    /// </summary>
    Task<SubIngredient> GetSubIngredientByIdAsync(int id);
    
    /// <summary>
    /// Creates a new subingredient
    /// </summary>
    Task<int> CreateSubIngredientAsync(SubIngredientDTO subingredient, int userId);
    
    /// <summary>
    /// Updates an existing subingredient
    /// </summary>
    Task<int> UpdateSubIngredientAsync(SubIngredient subingredient, int id);
    
    /// <summary>
    /// Deletes a subingredient
    /// </summary>
    Task<int> DeleteSubIngredientAsync(int id);
}