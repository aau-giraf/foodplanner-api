using FoodplannerDataAccessSql.Image;
using FoodplannerModels.Account;
using FoodplannerModels.Image;


namespace FoodplannerServices.Image;

public class FoodImageService(IImageService imageService, IFoodImageRepository foodImageRepository) : IFoodImageService
{
    public async Task<int> CreateFoodImage(int userid, Stream imageStream, string imageName, string imageType, long imageFileSize)
    {
        var imageId = await imageService.SaveImageAsync(userid, imageStream, imageType);
        int foodImageId = await foodImageRepository.InsertImageAsync(
            imageId.ToString(), 
            userid, 
            imageName, 
            imageType, 
            imageFileSize);
        
        return foodImageId;
    }

    public async Task<FoodImage> GetFoodImage(int foodImageId)
    {
        var foodImage = await foodImageRepository.GetImageByIdAsync(foodImageId);
        return foodImage;
    }

    public async Task<string> GetFoodImageLink(int foodImageId)
    {
        var foodImage = await foodImageRepository.GetImageByIdAsync(foodImageId);
        var foodImageLink = await imageService
            .LoadImagePresignedAsync(foodImage.UserId, Guid.Parse(foodImage.ImageId), foodImage.ImageFileType);
        if (foodImageLink != null)
        {
            throw new NullReferenceException("Image link could not be retrieved.");
        }
        return foodImageLink;
    }

    public async Task<bool> DeleteImage(int foodImageId)
    {
        var foodImage = await foodImageRepository.GetImageByIdAsync(foodImageId);
        await foodImageRepository.DeleteImageAsync(foodImageId);
        return await imageService.DeleteImageAsync(foodImage.UserId, Guid.Parse(foodImage.ImageId), foodImage.ImageFileType);
    }
}