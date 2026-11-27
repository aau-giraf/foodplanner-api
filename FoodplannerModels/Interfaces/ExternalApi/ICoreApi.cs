namespace FoodplannerModels.Interfaces.ExternalApi;

public interface ICoreApi
{
	Task<string> GetImages(List<string> imageIds);
	Task<string> DeleteImage();
}