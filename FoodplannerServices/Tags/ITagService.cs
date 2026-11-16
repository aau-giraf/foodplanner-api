using FoodplannerServices.Tags;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Tags;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Tags;

public interface ITagService
{
    Task<Tag> createTagId(Categories categories, IngredientDTO ingredient);
}