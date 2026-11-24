using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

namespace FoodplannerModels.Image;

public class SavePixabayImageDTO
{
    public required string Url { get; set; }
    public required string SearchWord { get; set; } 

    public required Ingredient ingredientId { get; set; }
    public required Category categoryIds { get; set; }

}