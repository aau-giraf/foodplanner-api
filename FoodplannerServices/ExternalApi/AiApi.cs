using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using FoodplannerModels.Interfaces.ExternalApi;
using FoodplannerModels.DTOs.ExternalApi.Ai;

namespace FoodplannerServices.ExternalApi;

public class AiApi(HttpClient httpClient, IConfiguration configuration) : IAiApi
{
	private readonly string BASE_URL = configuration["Infisical:AI_BASE_URL"]
		?? throw new InvalidOperationException("AI base URL is missing.");

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

		return imageId;
	}
}