namespace FoodplannerModels.Image;

public class GetPixabayImageDTO
{
    public string Language { get; set; }
    public required string SearchWord { get; set; } 
}