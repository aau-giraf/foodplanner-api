namespace FoodplannerModels.Lunchbox;

// Class for each ingredient in a meal.
// Contains the name of the ingredient and references to a user and an image.

public class Ingredient {
    // Gets an ingredient by ID asynchronously.
    public required int Id {get; set;}
    // Name of the ingredient.
    public required string Name {get; set;}
    // Reference to the user associated with the ingredient.
    public required string User_ref {get; set;}
    // Reference to the ingredient's image.
    public required string Image_ref {get; set;}
}