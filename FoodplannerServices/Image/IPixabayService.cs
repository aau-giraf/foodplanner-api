using FoodplannerModels.Image;

namespace FoodplannerServices.Image;

// Interface for Pixabay service
public interface IPixabayService
{
    public Task<Pixabay> GetPixabayImage (PixabayDTO search);
    public Task<int> SaveImageUrl(int userId, string keyword, string url);
}