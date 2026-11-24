using FoodplannerModels.Image;

namespace FoodplannerDataAccessSql.Image;

public interface IFoodImageRepository
{
    Task<FoodImage?> GetByIdAsync(int foodImageId);
    Task<int> InsertAsync(FoodImage entity);
    Task<int> DeleteAsync(int id);
    Task<IEnumerable<FoodImage>> GetAllStockImagesAsync();
}