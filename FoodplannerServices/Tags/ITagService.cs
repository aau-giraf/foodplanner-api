using FoodplannerServices.Tags;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

namespace FoodplannerServices.Tags;

public interface ITagService
{
    Task<TagDTO> createTagId(CategoryDTO categories, IngredientDTO ingredient);

    Task<TagDTO> GetImagesFromTags(string tags);
}