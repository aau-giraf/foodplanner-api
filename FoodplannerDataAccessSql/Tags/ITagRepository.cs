using FoodplannerModels;
using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

public interface ITagRepository : IGenericRepository<TagDTO>
{
    Task<bool> TagExistsAsync(int category_id, int ingredient_id);
    Task<IEnumerable<int>> GetImageIdsFromTagsAsync(List<string> tags);
}