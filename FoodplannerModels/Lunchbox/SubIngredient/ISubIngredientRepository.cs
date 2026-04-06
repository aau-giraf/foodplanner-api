namespace FoodplannerModels.Lunchbox;

/// <summary>
/// Repository interface for SubIngredient data access
/// </summary>
public interface ISubIngredientRepository
{
    /// <summary>
    /// Gets all subingredients (for testing/admin purposes)
    /// </summary>
    Task<IEnumerable<SubIngredient>> GetAllAsync();
    
    /// <summary>
    /// Gets all subingredients for a specific user
    /// </summary>
    Task<IEnumerable<SubIngredient>> GetAllByUserAsync(int userId);
    
    /// <summary>
    /// Gets a specific subingredient by ID
    /// </summary>
    Task<SubIngredient> GetByIdAsync(int id);
    
    /// <summary>
    /// Creates a new subingredient
    /// </summary>
    Task<int> InsertAsync(SubIngredientDTO entity, int userId);
    
    /// <summary>
    /// Updates an existing subingredient
    /// </summary>
    Task<int> UpdateAsync(SubIngredient entity, int id);
    
    /// <summary>
    /// Deletes a subingredient
    /// </summary>
    Task<int> DeleteAsync(int id);
}