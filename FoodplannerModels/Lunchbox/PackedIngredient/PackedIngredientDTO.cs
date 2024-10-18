namespace FoodplannerModels.Lunchbox;

/**
* Temperary class for the meal.
*/
public class PackedIngredientDTO {
    public required int Id {get; set;}
    public required string IngredientRef {get; set;}
    public required int MealRef {get; set;}
}