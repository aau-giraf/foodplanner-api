namespace FoodplannerModels.Lunchbox;

/// <summary>
/// Represents a subingredient that can be part of an ingredient
/// </summary>
public class SubIngredient
{
    /// <summary>
    /// Unique identifier for the subingredient
    /// </summary>
    public required int Id { get; set; }
    
    /// <summary>
    /// Name of the subingredient
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Reference to the user who owns this subingredient
    /// </summary>
    public required int User_id { get; set; }
    
    /// <summary>
    /// Optional reference to the subingredient's image
    /// </summary>
    public int? Food_image_id { get; set; }
}