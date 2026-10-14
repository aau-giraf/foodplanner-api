namespace FoodplannerModels.Lunchbox;

// Class for each ingredient in a meal.
// Contains the name of the ingredient and references to a user and an image.

public class IngredientDTO {
    public required int Id {get; set;}
    public required string Name {get; set;}
    public required int User_id {get; set;}
    public int? Food_image_id {get; set;}
}