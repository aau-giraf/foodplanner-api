using System.Text.Json;
using Microsoft.Extensions.Configuration;
using FoodplannerModels.Interfaces.ExternalApi;

namespace FoodplannerServices.ExternalApi;

public class CoreApi : ICoreApi
{
	private readonly string BASE_URL;
	private readonly HttpClient httpClient;
	private readonly IConfiguration configuration;

	public CoreApi(HttpClient httpClient, IConfiguration configuration)
	{
		BASE_URL = configuration["Infisical:CORE_BASE_URL"]
			?? throw new InvalidOperationException("CORE base URL is missing.");
		this.httpClient = httpClient;
		this.configuration = configuration;
	}

	public async Task<List<string>> GetImages(List<string> imageIds) {
		return null;
	}

	public async Task<string> DeleteImage() {
		return null;
	}
}