using FoodplannerModels.Account;
using FoodplannerModels.Images;
namespace FoodplannerServices.Image;





public class FoodImageService(IImageService imageService) 
{

    public async Task<bool> SaveFoodImage(int userid, Stream imageStream, string imageName, string imageType)
    {
        var imageId = await imageService.SaveImage(userid, imageStream);
        FoodImage foodplannerImage = new FoodImage(imageId.ToString(), userid, imageName, imageType, imageStream.Length);



        return true;
    }

    
    
    
    
}