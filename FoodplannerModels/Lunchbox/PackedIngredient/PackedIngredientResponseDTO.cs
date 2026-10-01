using System.Text.Json.Serialization;

namespace FoodplannerModels.Lunchbox;

/**
* Response shape for a packed ingredient inside a meal.
* Unlike PackedIngredientDTO (used for writes, where ingredient_id is an int FK),
* this nests the full ingredient under the "ingredient_id" key so clients get the
* ingredient's name and image without extra lookups.
*/
public class PackedIngredientResponseDTO
{
    public int Id { get; set; }
    public int Meal_id { get; set; }

    // Nested ingredient, serialized as "ingredient_id" for client compatibility.
    [JsonPropertyName("ingredient_id")]
    public IngredientDTO Ingredient { get; set; } = null!;

    public int order_number { get; set; }
}
