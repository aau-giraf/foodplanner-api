namespace FoodplannerModels.Lunchbox;

/**
* Class for each ingredient in a meal.
* Contains the name of the ingredient and references to a user and an image.
*/
public class Ingredient {
    public required int Id {get; set;}
    public required string Name {get; set;}
    public required string User_ref {get; set;}
    public required string Image_ref {get; set;}
}