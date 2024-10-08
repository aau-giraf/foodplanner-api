namespace foodplanner_api.Models;

/**
* Class for each element in a meal.
* Contains the name of the element and its attributes.
* Contains an image of the element and an alternate text in case the image cannot be loaded or is not shown for other reasons.
* Furthermore it is possible to descripe/comment on the ingredient, e.g. the preparation method.
*/
public class Ingredient {
    private string Name {get; set;}
    private string? Description {get; set;} // A description or comment on how the ingredient should be eaten/prepared.

    //private Image parameter
    private string AltText {get; set;} // Alternate text in case the image is not shown
    private string[] Attributes {get; set;} // Attributes describing the ingredient, e.g. soft, chruncy, hot, cold

    /**
    * @Name the name of the ingredient
    * @Image an image file related to the ingredient
    * @AltText text shown if the image is not loaded
    * @Attributes an array of attributes describing the ingredient
    * @return an instance of the Ingredient class
    */
    public Ingredient(string Name, /*Image Image,*/ string AltText, string[] Attributes){
        this.Name = Name;
        // this.Image = Image;
        this.AltText = AltText;
        this.Attributes = Attributes;
    }
}