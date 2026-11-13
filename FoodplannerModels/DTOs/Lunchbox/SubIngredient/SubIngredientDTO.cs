// Adds the SubIngredient class to the FoodplannerModels.Lunchbox namespace
namespace FoodplannerModels.Lunchbox;

// Represents a subingredient that can be part of an ingredient
public class SubIngredientDTO
{
    public required int Parent_id { get; set; }
    public required int Child_id { get; set; }
    public required int order_no { get; set; }
}