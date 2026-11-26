using System.Text.Json;
using Microsoft.Extensions.Configuration;
using FoodplannerModels.Interfaces.ExternalApi;

namespace FoodplannerServices.ExternalApi;

public class AiApi(HttpClient httpClient, IConfiguration configuration) : IAiApi
{
	private readonly string BASE_URL = configuration["Infisical:AI_BASE_URL"]
        ?? throw new InvalidOperationException("AI base URL is missing.");

	public async Task<string> CreateImage() {
		return null;
	}
}