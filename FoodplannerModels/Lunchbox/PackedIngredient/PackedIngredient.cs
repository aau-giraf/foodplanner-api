namespace FoodplannerModels.Lunchbox;

/**
* Class for the link between meals and ingredients in the foodplanner.
* Contains the references to a meal and an ingredient.
*/
public class PackedIngredient {
    // The unique identifier for the PackedIngredient entry.
    public required int Id {get; set;}
    // Reference to the Ingredient being packed (foreign key).
    public required int Ingredient_ref {get; set;}
    // Reference to the Meal that includes the ingredient (foreign key).
    public required int Meal_ref {get; set;}
}