namespace FoodplannerModels.Lunchbox;

/**
* Class for individual meals in the foodplanner.
* Contains the title of the Meal and references to a user and an image.
* The date of creation is also stored.
*/
public class Meal {
    // Unique identifier for the ingredient.
    public required int Id {get; set;}
    // Title of the meal.
    public required string Title {get; set;}
    // Reference to the user associated with the meal.
    public required string User_ref {get; set;}
    // Reference to the meal's image.
    public required string Image_ref {get; set;}
    // Date accosiated with the meal
    public required string Date {get; set;}
}