using System.Text.Json;
using AutoMapper;
using FoodplannerModels.Image;

namespace FoodplannerServices.ExternalApi;

public class PixabayApi(HttpClient httpClient, IMapper _mapper) : IPixabayApi
{
    private const string API_KEY = "API_KEY";

    public async Task<Pixabay> SearchImages(
        string language,
        string searchTerm,
        int perPage,
        string imageType,
        string order,
        string category)
    {
        var parameters = new List<string>();

        parameters.Add("key=" + API_KEY);
        parameters.Add("q=" + Uri.EscapeDataString(searchTerm));
        parameters.Add("lang=" + Uri.EscapeDataString(language));
        parameters.Add("per_page=" + perPage);
        parameters.Add("image_type=" + Uri.EscapeDataString(imageType));
        parameters.Add("order=" + Uri.EscapeDataString(order));
        parameters.Add("category=" + Uri.EscapeDataString(category));
        
        string url = "https://pixabay.com/api/?" + string.Join("&", parameters);
        string res = await httpClient.GetStringAsync(url);
        using JsonDocument data = JsonDocument.Parse(res);

        var imageURLs = new Pixabay()
        {
            Response = []
        };

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