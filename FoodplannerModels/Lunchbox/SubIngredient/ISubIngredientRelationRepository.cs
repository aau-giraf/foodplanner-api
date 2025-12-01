namespace FoodplannerModels.Lunchbox;

/// <summary>
/// Repository interface for SubIngredientRelation data access
/// </summary>
public interface ISubIngredientRelationRepository
{
    /// <summary>
    /// Gets all subingredient relations (for testing/admin purposes)
    /// </summary>
    Task<IEnumerable<SubIngredientRelation>> GetAllAsync();
    
    /// <summary>
    /// Gets all subingredient relations for a specific ingredient
    /// </summary>
    Task<IEnumerable<SubIngredientRelation>> GetAllByIngredientIdAsync(int ingredientId);
    
    /// <summary>
    /// Gets a specific subingredient relation by ID
    /// </summary>
    Task<SubIngredientRelation> GetByIdAsync(int id);
    
    /// <summary>
    /// Creates a new subingredient relation
    /// </summary>
    Task<int> InsertAsync(int ingredientId, int subingredientId);
    
    /// <summary>
    /// Updates an existing subingredient relation
    /// </summary>
    Task<int> UpdateAsync(SubIngredientRelation entity, int id);
    
    /// <summary>
    /// Deletes a subingredient relation
    /// </summary>
    Task<int> DeleteAsync(int id);
    
    /// <summary>
    /// Updates the order of a subingredient within an ingredient
    /// </summary>
    Task<bool> UpdateOrderAsync(int id, int order);
}