namespace FoodplannerModels.Lunchbox;

/// <summary>
/// DTO for displaying a subingredient relation with full subingredient details
/// </summary>
public class SubIngredientRelationDTO
{
    /// <summary>
    /// Unique identifier for the relation
    /// </summary>
    public required int Id { get; set; }
    
    /// <summary>
    /// Reference to the ingredient
    /// </summary>
    public required int Ingredient_id { get; set; }
    
    /// <summary>
    /// Full subingredient object
    /// </summary>
    public required SubIngredient Subingredient_id { get; set; }
    
    /// <summary>
    /// Order of the subingredient within the ingredient
    /// </summary>
    public required int Order_number { get; set; }
}