using FoodplannerModels;
using FoodplannerModels.Image;

namespace FoodplannerDataAccessSql.Image;

public interface IFoodImageRepository : IGenericRepository<FoodImage>
{
    public  Task<IEnumerable<FoodImage>> GetAllStockImagesAsync();
}