namespace FoodplannerModels.Interfaces.ExternalApi;

public interface ICoreApi
{
	Task<List<string>> GetImages(List<string> imageIds);
	Task<string> DeleteImage();
}