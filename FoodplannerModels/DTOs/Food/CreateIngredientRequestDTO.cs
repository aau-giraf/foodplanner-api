using FoodplannerModels.Lunchbox;

namespace FoodplannerModels.DTOs.Food;

public class CreateIngredientRequestDTO
{
    public required Ingredient Ingredient { get; set; }
    public int[] CategoryIds { get; set; } = [];
}