using System.Text.Json;
using Microsoft.Extensions.Configuration;
using FoodplannerModels.Interfaces.ExternalApi;

namespace FoodplannerServices.ExternalApi;

public class CoreApi(HttpClient httpClient, IConfiguration configuration) : ICoreApi
{
	private readonly string BASE_URL = configuration["Infisical:CORE_BASE_URL"]
		?? throw new InvalidOperationException("CORE base URL is missing.");

	public async Task<List<string>> GetImages(List<string> imageIds) {
		return null;
	}

	public async Task<string> DeleteImage() {
		return null;
	}
}