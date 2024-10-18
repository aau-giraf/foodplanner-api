namespace FoodplannerModels.Lunchbox;

/**
* Class for individual meals in the foodplanner.
* Contains the title of the Meal and references to a user and an image.
* The date of creation is also stored.
*/
public class Meal {
    public required int Id {get; set;}
    public required string Title {get; set;}
    public required string User_ref {get; set;}
    public required string Image_ref {get; set;}
    public required string Date {get; set;}
}