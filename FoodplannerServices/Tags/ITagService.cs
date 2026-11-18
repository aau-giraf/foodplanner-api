using FoodplannerServices.Tags;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Tags;

public interface ITagService
{
    Task<TagDTO> createTagId(CategoryDTO categories, IngredientDTO ingredient);

    Task<TagDTO> GetImagesFromTags(string tags);
}