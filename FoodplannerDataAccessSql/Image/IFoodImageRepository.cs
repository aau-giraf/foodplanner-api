using FoodplannerModels.Image;

namespace FoodplannerDataAccessSql.Image;

public interface IFoodImageRepository
{
    Task<IEnumerable<FoodImage>> GetAllImagesAsync();
    Task<FoodImage> GetImageByIdAsync(int userId, String imageId);
    Task<int> InsertImageAsync(string foodImage, int userid, string imageName, string imageType, long imageStreamLength);
    Task<int> DeleteImageAsync(int imageId);
}