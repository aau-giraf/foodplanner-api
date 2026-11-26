namespace FoodplannerModels.Interfaces.ExternalApi;

public interface ICoreApi
{
	Task<string> GetImages();
	Task<string> DeleteImage();
}