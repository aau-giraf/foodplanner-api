namespace FoodplannerModels.Lunchbox;

/**
* Class for the link between meals and ingredients in the foodplanner.
* Contains the references to a meal and an ingredient.
*/
public class PackedIngredient {
    public required int Id {get; set;}
    public required string IngredientRef {get; set;}
    public required int MealRef {get; set;}
}