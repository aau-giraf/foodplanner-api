using FoodplannerModels.DTOs.ExternalApi.Ai;

namespace FoodplannerModels.Interfaces.ExternalApi;

public interface ICoreApi
{
	Task<string> DeleteImage(DeleteImageDTO deleteImageDTO);
	Task<string> GetImages(List<string> imageIds);
}