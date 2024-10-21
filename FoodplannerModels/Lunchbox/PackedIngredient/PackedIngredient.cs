namespace FoodplannerModels.Lunchbox;

/**
* Class for the link between meals and ingredients in the foodplanner.
* Contains the references to a meal and an ingredient.
*/
public class PackedIngredient {
    public required int Id {get; set;}
    public required int Ingredient_ref {get; set;}
    public required int Meal_ref {get; set;}
}