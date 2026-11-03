using AutoMapper;
using FoodplannerModels.Image;
using FoodplannerApi.PixabayApi;



namespace FoodplannerServices.Image;

// Service implementation for Pixabay business logic
public class PixabayService() : IPixabayService
{
    // Method for retrieving a list of pixabay images 
    public async Task<List<string>> GetPixabayImage (string language, string searchWord) 
    {
        // Method from Pixabay API
        return
    }

}

