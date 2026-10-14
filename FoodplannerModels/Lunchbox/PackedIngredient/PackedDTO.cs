// Adds the PackedDTO class to the FoodplannerModels.Lunchbox namespace
namespace FoodplannerModels.Lunchbox;

public class PackedDTO {
    // Reference to the Ingredient being packed (foreign key).
    public required int Ingredient_id { get; set; }
    // Reference to the Meal that includes the ingredient (foreign key).
    public required int Meal_id { get; set; }
    public int Order_number { get; set; }
}