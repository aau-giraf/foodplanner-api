namespace FoodplannerModels.Lunchbox;

/**
* Class for each element in a meal.
* Contains the name of the element and its attributes.
* Contains an image of the element and an alternate text in case the image cannot be loaded or is not shown for other reasons.
* Furthermore it is possible to descripe/comment on the ingredient, e.g. the preparation method.
*/
public class Ingredient {
    public required string Name {get; set;}
    //public string? Description {get; set;} // A description or comment on how the ingredient should be eaten/prepared.
    //private Image parameter
    //public string AltText {get; set;} // Alternate text in case the image is not shown
    //public string[] Attributes {get; set;} // Attributes describing the ingredient, e.g. soft, chruncy, hot, cold
}