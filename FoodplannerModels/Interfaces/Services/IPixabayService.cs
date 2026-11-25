using FoodplannerModels.Image;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

namespace FoodplannerServices.Image;

// Interface for Pixabay service
public interface IPixabayService
{
    public Task<Pixabay> GetPixabayImage (GetPixabayImageDTO search);
    public Task<int> SavePixabayUrl(int userId, string keyword, string url );
}