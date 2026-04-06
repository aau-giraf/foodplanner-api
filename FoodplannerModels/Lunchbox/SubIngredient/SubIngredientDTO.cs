namespace FoodplannerModels.Lunchbox;

/// <summary>
/// DTO for creating a subingredient (without ID and user_id)
/// </summary>
public class SubIngredientDTO
{
    /// <summary>
    /// Name of the subingredient
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Optional reference to the subingredient's image
    /// </summary>
    public int? Food_image_id { get; set; }
}