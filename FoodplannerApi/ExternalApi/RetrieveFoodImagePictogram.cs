using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FoodplannerApi.ExternalApi;

public class PixabayAPI
{
    private const string API_KEY = "API_KEY";

    private static readonly HttpClient client = new HttpClient();

    public static async Task<List<string>> SearchImages(
        string searchTerm,
		string language = "da",
        int perPage = 20,
        string imageType = "all",
		string order = "popular",
        string category = "food")
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
        string res = await client.GetStringAsync(url);

        JObject data = JObject.Parse(res);

        List<string> results = new List<string>();

        if ((int)data["totalHits"] > 0)
        {
            foreach (var hit in data["hits"])
            {
                results.Add((string)hit["webformatURL"]);
            }
        }

        return results;
    }
}