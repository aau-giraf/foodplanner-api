using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

// Adds Interface TagService to the class to the FoodplannerServices.Tags namespace
namespace FoodplannerServices.Tags;

// Interface for Tag Service 
public interface ITagService
{
    Task<int> CreateTag(int category_id, int ingredient_id);

    Task<IEnumerable<FoodImageDTO>> GetImagesFromTags(string tags);
}