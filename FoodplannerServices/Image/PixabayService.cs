using FoodplannerModels.Image;
using Microsoft.AspNetCore.Mvc;
using FoodplannerServices.ExternalApi;
using FoodplannerServices.Image;
using FoodplannerModels.Tags;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using FoodplannerModels.Lunchbox;


namespace FoodplannerServices.Image;

// Service implementation for Pixabay business logic
public class PixabayService(IPixabayApi pixabayApi, IFoodImageService foodImageService, IWebClientWrapper webClientWrapper, ITagService tagService) : IPixabayService
{
    // Max image size is 100 MB
    private readonly long _maxFileSize = 100000000; 

    // Method from Pixabay API
    public async Task<Pixabay> GetPixabayImage (GetPixabayImageDTO search){ 
        if (search.Language == null)
        {
            search.Language = "da";
        }
        var images = await pixabayApi.SearchImages(search.Language, search.SearchWord, 20, "all", "popular", "food");
        return images;
    }

    public async Task<int> SavePixabayUrl (int userid, string keyword, string url)
    {
        // Convert the pixabay url to a stream
        byte[] imageStream = webClientWrapper.DownloadData(url);
        var image = new MemoryStream(imageStream);

        // Check if image is empty or exceeds the maximum image size
        if (image.Length == 0)
        {
            throw new Exception("File is empty");
        }
        if (image.Length >= _maxFileSize)
        {
            throw new Exception("File is too big");
        }

        // Return the id of the created image
        return await foodImageService.CreateFoodImage(userid, image, keyword, "image/jpeg", image.Length);      
    }

}

