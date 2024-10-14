namespace FoodplannerModels.Lunchbox;

/**
* Class for individual meals in the foodplanner.
* Contains the name of the Meal and description for commenting/describing the Meal
* Contains an image of the element and an alternate text in case the image cannot be loaded or is not shown for other reasons.
*/
public class Meal {
    //public int Id{get; set;}
    public required string Meal_name {get; set;}
    public string? Description {get; set;} // A description or comment on how the meal should be eaten/prepared.
    //public LinkedList<Ingredient> Ingredients {get; set;}
    //private Image parameter
    public string? AltText {get; set;} // Alternate text in case the image is not shown
    public required string Ingredient {get; set;}
}