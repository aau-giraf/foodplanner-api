namespace FoodplannerModels.Interfaces.ExternalApi;

public interface IAiApi
{
	Task<string> CreateImage();
}