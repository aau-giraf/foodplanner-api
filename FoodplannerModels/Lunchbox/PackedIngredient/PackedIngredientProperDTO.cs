namespace FoodplannerModels.Lunchbox;

/**
* Class for the link between meals and ingredients in the foodplanner.
* Contains the references to a meal and an ingredient.
*/
public class PackedIngredientProperDTO
{
    // Reference to the Meal that includes the ingredient.
    public required int Meal_id { get; set; }
    // Reference to the Ingredient being packed.
    public required int Ingredient_id { get; set; }
    // The unique identifier for the PackedIngredient entry.
}