using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

// Adds Interface TagService to the class to the FoodplannerServices.Tags namespace
namespace FoodplannerServices.Tags;

// Interface for Tag Service 
public interface ITagService
{
    Task<TagDTO> createTag(CategoryDTO category, IngredientDTO ingredient);

    Task<IEnumerable<FoodImageDTO>> GetImagesFromTags(string tags);
}