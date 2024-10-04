namespace FoodplannerModels.Lunchbox;

/**
* Temperary class for the meal.
*/
public class Meal
{
    public required string Meal_name { get; set; }
    public required LinkedList<Ingredient> Meal_ingredients { get; set; }
}