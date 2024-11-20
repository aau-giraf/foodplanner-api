namespace FoodplannerModels.Lunchbox;

/**
* Class for individual meals in the foodplanner.
* Contains the title of the Meal and references to a user and an image.
* The date of creation is also stored.
*/
public class MealDTO {
    // Unique identifier for the ingredient.
    public required int Id {get; set;}
    // Reference to the meal's image.
    public int? Image_ref {get; set;}
    // Title of the meal.
    public required string Title {get; set;}
    // Date accosiated with the meal
    public required string Date {get; set;}
    //List of ingredients in the meal
    public required List<PackedIngredientDTO> Ingredients {get; set;}
}