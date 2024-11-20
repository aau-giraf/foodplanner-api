namespace FoodplannerModels.Lunchbox;

// Class for each ingredient in a meal.
// Contains the name of the ingredient and references to a user and an image.

public class Ingredient {
    // Name of the ingredient.
    public required string Name {get; set;}
    // Reference to the ingredient's image.
    public int? Food_image_id {get; set;}
}