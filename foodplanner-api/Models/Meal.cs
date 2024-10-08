namespace foodplanner_api.Models;

/**
* Class for individual meals in the foodplanner.
* Contains the name of the Meal and description for commenting/describing the Meal
* Contains an image of the element and an alternate text in case the image cannot be loaded or is not shown for other reasons.
*/
public class Meal {
    private string Name {get; set;}
    private string? Description {get; set;} // A description or comment on how the meal should be eaten/prepared.
    private LinkedList<Ingredient> Ingredients {get; set;}
    //private Image parameter
    private string AltText {get; set;} // Alternate text in case the image is not shown

    /**
    * @Name the name of the meal
    * @Image an image file related to the meal
    * @AltText text shown if the image is not loaded
    * @returns an instance of the Meal class
    */
    public Meal(string Name, /*Image Image,*/ string AltText) {
        this.Name = Name;
        // this.Image = Image;
        this.AltText = AltText;
    }
}