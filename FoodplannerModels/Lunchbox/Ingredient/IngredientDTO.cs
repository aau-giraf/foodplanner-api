using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Lunchbox;

// Class for each ingredient in a meal.
// Contains the name of the ingredient and references to a user and an image.

public class IngredientDTO
{
    [Required]
    public required int Id { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    [Required]
    public required int User_id {get; set;}
    
    [Required]
    public int? Food_image_id { get; set; }
    
    
}