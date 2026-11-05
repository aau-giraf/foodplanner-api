using System.Text.Json;
using AutoMapper;
using FoodplannerModels.Image;
using Microsoft.Extensions.Configuration;

namespace FoodplannerServices.ExternalApi;

public class PixabayApi(HttpClient httpClient, IConfiguration configuration) : IPixabayApi
{
    // Define the API key and URL for the Pixabay API, loaded from appetsettings.Development.json
    private readonly string API_KEY = configuration["Infisical:PIXABAY_API_KEY"];
    private readonly string API_URL = configuration["Infisical:PIXABAY_API_URL"];

    public async Task<Pixabay> SearchImages(
        string language,
        string searchTerm,
        int perPage,
        string imageType,
        string order,
        string category)
    {
        // Verify that the API key and URL are not null or empty
        if (string.IsNullOrWhiteSpace(API_KEY) || string.IsNullOrWhiteSpace(API_URL))
        {
            throw new ApplicationException("Pixabay API key or URL is not configured.");
        }

        // Build the query parameters for the Pixabay API request
        var parameters = new List<string>();
        parameters.Add("key=" + API_KEY);
        parameters.Add("q=" + Uri.EscapeDataString(searchTerm));
        parameters.Add("lang=" + Uri.EscapeDataString(language));
        parameters.Add("per_page=" + perPage);
        parameters.Add("image_type=" + Uri.EscapeDataString(imageType));
        parameters.Add("order=" + Uri.EscapeDataString(order));
        parameters.Add("category=" + Uri.EscapeDataString(category));
        
        // Construct the full URL for the Pixabay API request
        string url = $"{API_URL}?" + string.Join("&", parameters);
        string res = await httpClient.GetStringAsync(url);
        using JsonDocument data = JsonDocument.Parse(res);

        // Create a new Pixabay object to hold the image URLs
        var imageURLs = new Pixabay()
        {
            Response = []
        };

        // Check if there are any hits in the response and add the image URLs to the Pixabay object
        if (data.RootElement.GetProperty("totalHits").GetInt32() > 0)
        {
            foreach (JsonElement hit in data.RootElement.GetProperty("hits").EnumerateArray())
            {
                imageURLs.Response.Add(hit.GetProperty("webformatURL").GetString()!);
            }
        }

        return imageURLs;
    }

    
}