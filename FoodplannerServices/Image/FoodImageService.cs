using FoodplannerDataAccessSql.Image;
using FoodplannerModels.Account;
using FoodplannerModels.Image;


namespace FoodplannerServices.Image;

public class FoodImageService(IImageService imageService, IFoodImageRepository foodImageRepository) 
{
    
    public async Task<string> SaveFoodImage(int userid, Stream imageStream, string imageName, string imageType)
    {
        try
        {
            var imageId = await imageService.SaveImageAsync(userid, imageStream);
            FoodImageDTO foodImage = new FoodImageDTO(imageId.ToString(), userid, imageName, imageType, imageStream.Length);
            string foodImageId = await foodImageRepository.InsertImageAsync(foodPlannerImage);
            
            return foodImageId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> GetFoodImage(int userId, Guid imageId)
    {
        var foodImage = foodImageRepository.GetImageByIdAsync();
        Guid imageGuid = Guid.Parse(foodImage.ImageId);
        var imageStream = imageService.LoadImageAsync(imageGuid);

        return true;
    }

    public async Task<bool> DeleteImage(User user, string imageId)
    {
        var foodImage = _foodImageRepository.GetImageByIdAsync(imageId);
        
        //var foodImageGUID = new Guid(foodImage.ImageId);
        Guid imageGuid = Guid.Parse(foodImage.ImageId);
        await imageService.DeleteImageAsync(user.Id, imageGuid); 
        await _foodImageRepository.DeleteImageAsync(foodImage.Id);
        return true;
    }
}