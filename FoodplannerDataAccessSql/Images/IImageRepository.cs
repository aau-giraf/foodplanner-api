using System.Net.Mime;
using FoodplannerModels.Images;

namespace FoodplannerDataAccessSql.Images;

public interface IImageRepository
{


    Task<IEnumerable<FoodImageDTO>> GetAllImagesAsync();
    Task<FoodImageDTO> GetImageByIdAsync(int imageId);
    Task<int> SaveImageAsync(int imageId);
    Task<int> UpdateImageAsync(FoodImageDTO image);
    Task<int> DeleteImageAsync(int imageId);
    
    
}