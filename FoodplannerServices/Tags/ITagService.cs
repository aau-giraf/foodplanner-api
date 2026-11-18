using FoodplannerServices.Tags;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Tags;

public interface ITagService
{
    Task<TagDTO> createTagId(CategoryDTO category, IngredientDTO ingredient);

    Task<IEnumerable<FoodImageDTO>> GetImagesFromTags(string tags);
}