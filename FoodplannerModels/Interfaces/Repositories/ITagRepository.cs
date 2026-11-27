using FoodplannerModels;
using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

// Interface for Tag repository
public interface ITagRepository
{
    Task<int> InsertAsync(Tag entity);
    Task<bool> TagExistsAsync(int category_id, int ingredient_id);
    Task<IEnumerable<int>> GetImageIdsFromTagsAsync(List<string> tags);
}