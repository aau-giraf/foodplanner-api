using AutoMapper;
using FoodplannerModels.Image;
using Microsoft.AspNetCore.Mvc;
using FoodplannerServices.ExternalApi;


namespace FoodplannerServices.Image;

// Service implementation for Pixabay business logic
public class PixabayService(IPixabayApi pixabayApi, IMapper _mapper) : IPixabayService
{
    // Method for retrieving a list of pixabay images 

    // Method from Pixabay API
    public async Task<Pixabay> GetPixabayImage (PixabayDTO search){ 
        if (search.Language == null)
        {
            search.Language = "da";
        }
        var images = await pixabayApi.SearchImages(search.Language, search.SearchWord, 20, "all", "popular", "food");
        return images;
    }
    
}

