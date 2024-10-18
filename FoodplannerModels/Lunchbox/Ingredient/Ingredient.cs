namespace FoodplannerModels.Lunchbox;

/**
* Class for each ingredient in a meal.
* Contains the name of the ingredient and references to a user and an image.
*/
public class Ingredient {
    public required string Name {get; set;}
    public required string UserRef {get; set;}
    public required string ImageRef {get; set;}
}