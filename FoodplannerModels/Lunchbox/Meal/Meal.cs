namespace FoodplannerModels.Lunchbox;

/**
* Class for individual meals in the foodplanner.
* Contains the title of the Meal and references to a user and an image.
* The date of creation is also stored.
*/
public class Meal {
    public required int Id {get; set;}
    public required string Title {get; set;}
    public required string UserRef {get; set;}
    public required string IngredientRef {get; set;}
    public required DateOnly Date {get; set;}
}