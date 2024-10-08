using FoodplannerDataAccessSql.Images;
using FoodplannerModels.Account;
using FoodplannerModels.Images;




namespace FoodplannerServices.Image;

public class FoodImageService(IImageService imageService) 
{
    
    private ImageRepository imageRepository;
    
    public async Task<string> SaveFoodImage(int userid, Stream imageStream, string imageName, string imageType)
    {
        try
        {
            var imageId = await imageService.SaveImage(userid, imageStream);
            FoodImageDTO foodPlannerImage = new FoodImageDTO(imageId.ToString(), userid, imageName, imageType, imageStream.Length);
            string foodImageId = await imageRepository.SaveImageAsync(foodPlannerImage);
            
            return foodImageId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> GetImageByUserId(User user)
    {
        var foodImage = imageRepository.GetImageByIdAsync(user.Id);
        Guid imageGuid = Guid.Parse(foodImage.ImageId);
        var imageStream = imageService.GetImage(imageGuid);

        return true;
    }

    public async Task<bool> DeleteImage(User user, string imageId)
    {
        var foodImage = imageRepository.GetImageByIdAsync(imageId);
        
      
        
        
        //var foodImageGUID = new Guid(foodImage.ImageId);
        Guid imageGuid = Guid.Parse(foodImage.ImageId);
        await imageService.DeleteImage(user.Id, imageGuid); 
        await imageRepository.DeleteImageAsync(foodImage.Id);
        return true;
    }
    
    
    
    
}