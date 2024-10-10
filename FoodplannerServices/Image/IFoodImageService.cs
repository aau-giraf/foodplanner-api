using FoodplannerModels.Image;

namespace FoodplannerServices.Image;

public interface IFoodImageService
{
    public Task<int> CreateFoodImage(int userid, Stream imageStream, string imageName, string imageType);
    public Task<FoodImage> GetFoodImage(int foodImageId);

    public Task<bool> DeleteImage(int foodImageId);
}