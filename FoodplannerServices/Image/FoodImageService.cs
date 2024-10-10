using FoodplannerDataAccessSql.Image;
using FoodplannerModels.Account;
using FoodplannerModels.Image;


namespace FoodplannerServices.Image;

public class FoodImageService(IImageService imageService, IFoodImageRepository foodImageRepository) : IFoodImageService
{
    public async Task<int> CreateFoodImage(int userid, Stream imageStream, string imageName, string imageType)
    {
        var imageId = await imageService.SaveImageAsync(userid, imageStream, imageType);
        int foodImageId = await foodImageRepository.InsertImageAsync(imageId.ToString(), userid, imageName, imageType, imageStream.Length);
        
        return foodImageId;
    }

    public async Task<FoodImage> GetFoodImage(int foodImageId)
    {
        var foodImage = await foodImageRepository.GetImageByIdAsync(foodImageId);
        return foodImage;
    }

    public async Task<bool> DeleteImage(int foodImageId)
    {
        await foodImageRepository.DeleteImageAsync(foodImageId);
        return true;
    }
}