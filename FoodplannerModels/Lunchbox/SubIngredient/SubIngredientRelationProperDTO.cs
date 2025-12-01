namespace FoodplannerModels.Lunchbox;

/// <summary>
/// DTO for creating/updating a subingredient relation (just IDs)
/// </summary>
public class SubIngredientRelationProperDTO
{
    /// <summary>
    /// Reference to the ingredient
    /// </summary>
    public required int Ingredient_id { get; set; }
    
    /// <summary>
    /// Reference to the subingredient
    /// </summary>
    public required int Subingredient_id { get; set; }
}