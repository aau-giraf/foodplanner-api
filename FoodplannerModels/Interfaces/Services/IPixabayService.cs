using FoodplannerModels.Image;

namespace FoodplannerServices.Image;

// Interface for Pixabay service
public interface IPixabayService
{
    public Task<Pixabay> GetPixabayImage (GetPixabayImageDTO search);
    public Task<int> SavePixabayUrl(int userId, string keyword, string url);
}