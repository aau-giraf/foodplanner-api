namespace FoodplannerModels.Lunchbox;

/// <summary>
/// Service interface for SubIngredientRelation business logic
/// </summary>
public interface ISubIngredientRelationService
{
    /// <summary>
    /// Gets all subingredient relations (for admin purposes)
    /// </summary>
    Task<IEnumerable<SubIngredientRelationProperDTO>> GetAllSubIngredientRelationsAsync();
    
    /// <summary>
    /// Gets all subingredient relations for a specific ingredient with full details
    /// </summary>
    Task<IEnumerable<SubIngredientRelationDTO>> GetAllSubIngredientRelationsByIngredientIdAsync(int ingredientId);
    
    /// <summary>
    /// Gets a specific subingredient relation by ID
    /// </summary>
    Task<SubIngredientRelation> GetSubIngredientRelationByIdAsync(int id);
    
    /// <summary>
    /// Creates a new subingredient relation
    /// </summary>
    Task<int> CreateSubIngredientRelationAsync(int ingredientId, int subingredientId);
    
    /// <summary>
    /// Updates an existing subingredient relation
    /// </summary>
    Task<int> UpdateSubIngredientRelationAsync(SubIngredientRelation relation, int id);
    
    /// <summary>
    /// Deletes a subingredient relation
    /// </summary>
    Task<int> DeleteSubIngredientRelationAsync(int id);
    
    /// <summary>
    /// Updates the order of subingredient relations
    /// </summary>
    Task<bool> UpdateSubIngredientRelationOrderAsync(List<SubIngredientRelation> relations);
}