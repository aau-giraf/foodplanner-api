using FoodplannerServices.Tags;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

namespace FoodplannerServices.Tags;

public interface ITagService
{
    Task<TagDTO> CreateTagId(CategoryDTO category, IngredientDTO ingredient);

    Task<IEnumerable<FoodImageDTO>> GetImagesFromTags(string tags);
}