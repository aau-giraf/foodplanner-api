using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using FoodplannerModels.Interfaces.ExternalApi;
using FoodplannerModels.DTOs.ExternalApi.Ai;

namespace FoodplannerServices.ExternalApi;

public class CoreApi(HttpClient httpClient, IConfiguration configuration) : ICoreApi
{
	private readonly string BASE_URL = configuration["Infisical:CORE_BASE_URL"]
		?? throw new InvalidOperationException("CORE base URL is missing.");

	public async Task<List<string>> GetImages(List<string> imageIds) {
		return new List<string> {"hello"};
	}

	public async Task<string> DeleteImage(DeleteImageDTO deleteImageDTO)
	{
		// Generate request body
		using StringContent jsonContent = new(
			JsonSerializer.Serialize(deleteImageDTO),
			Encoding.UTF8,
			"application/json"
		);

		// Send request to CORES's API
		string url = $"{BASE_URL}/delete-image";
		using HttpResponseMessage res = await httpClient.PostAsync(
			url, 
			jsonContent
		);

		// Check if the request was successful
		if (!res.IsSuccessStatusCode)
		{
			throw new HttpRequestException($"Request to CORE API failed with status code: {res.StatusCode}");
		}

		return "deleted image with UUID: " + deleteImageDTO.ImageId;
	}
}