using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using FoodplannerModels.Interfaces.ExternalApi;
using FoodplannerModels.DTOs.ExternalApi.Ai;

namespace FoodplannerServices.ExternalApi;

public class AiApi : IAiApi
{
	private readonly string BASE_URL;
	private readonly HttpClient httpClient;
	private readonly IConfiguration configuration;
	private readonly CoreApi coreApi;
	
	AiApi(HttpClient httpClient, IConfiguration configuration, CoreApi coreApi)
	{
		BASE_URL = configuration["Infisical:AI_BASE_URL"]
			?? throw new InvalidOperationException("AI base URL is missing.");
		this.httpClient = httpClient;
		this.configuration = configuration;
		this.coreApi = coreApi;
	}

	public async Task<string> CreateImage(int userId, string prompt) {
		// Generate UUID v7 for ImgId
		var imageId = Guid.CreateVersion7(DateTimeOffset.UtcNow).ToString();

		// Generate request body
		using StringContent jsonContent = new(
			JsonSerializer.Serialize(new CreateImageDTO
			{
				Prompt = prompt,
				MapId = userId,
				Type = "CitizenID",
				ImgId = imageId,
				OriginApp = "FP"
			}),
			Encoding.UTF8,
			"application/json"
		);

		// Send request to AI's API
		string url = $"{BASE_URL}/create-image";
		using HttpResponseMessage res = await httpClient.PostAsync(
			url, 
			jsonContent
		);

		// Check if the request was successful
		if (!res.IsSuccessStatusCode)
		{
			throw new HttpRequestException($"Request to AI API failed with status code: {res.StatusCode}");
		}

		// Wait 4 sec before calling CORE
		await Task.Delay(4000);

		// Poll CORE API for the image MAX 5 times with 1 sec delay between each call
		var image = new List<string>();
		for (int i = 0; i < 5; i++)
		{
			await Task.Delay(1000);

			// Get image form CORE API
			var coreRes = await coreApi.GetImages(new List<string> { imageId });

			if (coreRes == null)
			{
				Console.WriteLine($"Image not ready yet. Attempt {i + 1}/5");
				continue;
			}
			
			image.Add(coreRes);
			break;
		}
		
		// If the image is still not available after 5 attempts, throw an exception
		if (image.Count == 0)
		{
			throw new InvalidOperationException("Failed to retrieve image after multiple attempts.");
		}
        return image[0];
	}
}