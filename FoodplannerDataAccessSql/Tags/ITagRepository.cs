using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

public interface ITagRepository
{
    Task<IEnumerable<int>> GetImageIdsFromTags(List<string> tags);
}