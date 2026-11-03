using FoodplannerModels.Image;


namespace FoodplannerServices.Image;

// Interface for Pixabay service
public interface IPixabayService
{
    public Task<List<string>> GetPixabayImage (string language, string searchWord);
}