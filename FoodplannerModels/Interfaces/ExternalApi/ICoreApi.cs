using FoodplannerModels.DTOs.ExternalApi.Ai;

namespace FoodplannerModels.Interfaces.ExternalApi;

public interface ICoreApi
{
	Task<string> DeleteImage(DeleteImageDTO deleteImageDTO);
	Task<List<string>> GetImages(List<string> imageIds);
}