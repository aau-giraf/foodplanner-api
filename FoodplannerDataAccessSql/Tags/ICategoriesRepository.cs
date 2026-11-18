using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

public interface IChildrenRepository
{
Task<IEnumerable<Categories>> GetAllAsync();

Task<Categories> InsertAsync(Categories categories);


}