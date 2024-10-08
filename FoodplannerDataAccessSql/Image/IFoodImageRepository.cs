using FoodplannerModels.Image;

namespace FoodplannerDataAccessSql.Image;

public interface IFoodImageRepository
{
    Task<IEnumerable<FoodImage>> GetAllImagesAsync();
    Task<FoodImage> GetImageByIdAsync(String imageId);
    Task<string> InsertImageAsync(FoodImage foodImage);
    Task<int> DeleteImageAsync(int imageId);
}