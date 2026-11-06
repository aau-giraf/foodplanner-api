using AutoMapper;
using FoodplannerModels.Image;
using Microsoft.AspNetCore.Mvc;
using FoodplannerServices.ExternalApi;
using FoodplannerDataAccessSql.Image;
using FoodplannerServices.Image;


namespace FoodplannerServices.Image;

// Service implementation for Pixabay business logic
public class PixabayService(IPixabayApi pixabayApi, IFoodImageService foodImageService) : IPixabayService
{
    // Method for retrieving a list of pixabay images 

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
        byte[] imageStream = null;
        // Convert the pixabay url to a stream 
        using ( var pixabay = new System.Net.WebClient())
        imageStream = pixabay.DownloadData(url);
        var image = new MemoryStream(imageStream);

        return await foodImageService.CreateFoodImage(userid, image, keyword, "image/jpeg", image.Length);
       
    }

}

