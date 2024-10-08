using System.Net.Mime;
using FoodplannerModels.Images;

namespace FoodplannerDataAccessSql.Images;

public interface IImageRepository
{
    Task<IEnumerable<FoodImageDTO>> GetAllImagesAsync();
    Task<FoodImageDTO> GetImageByIdAsync(int imageId);
    Task<FoodImageDTO> SaveImageAsync(FoodImageDTO foodImage);
    Task<int> UpdateImageAsync(FoodImageDTO image);
    Task<int> DeleteImageAsync(int imageId);
}