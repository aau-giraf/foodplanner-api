using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

namespace FoodplannerServices.Tags;

public class TagService()
{
    public async Task<Tag> CreateTagId (Categories categories, IngredientDTO ingredient)
    {
          return new Tag
          {
              Category_id = categories.Id,
              Ingredient_id = ingredient.Id
          };
    }

    // Get all stock images that match tags
    public async Task<TagDTO> GetImagesFromTags(string tags)
    {
        
    }
    
}