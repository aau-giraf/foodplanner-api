namespace FoodplannerModels.Lunchbox;

/**
* Response shape for a meal, including its packed ingredients with nested
* ingredient details. Used by the read endpoints; MealDTO remains the write shape.
*/
public class MealResponseDTO
{
    public int Id { get; set; }
    public int? Food_image_id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Date { get; set; } = string.Empty;
    public bool Template { get; set; }
    public IEnumerable<PackedIngredientResponseDTO> Ingredients { get; set; } = [];
}
