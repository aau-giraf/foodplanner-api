namespace FoodplannerModels.Lunchbox;

/// <summary>
/// Represents the many-to-many relationship between ingredients and subingredients
/// </summary>
public class SubIngredientRelation
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
    /// Reference to the subingredient
    /// </summary>
    public required int Subingredient_id { get; set; }
    
    /// <summary>
    /// Order of the subingredient within the ingredient
    /// </summary>
    public required int Order_number { get; set; }
}