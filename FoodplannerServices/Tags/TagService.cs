using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

namespace FoodplannerServices.Tags;

public class TagService()
{
    public async Task<TagDTO> CreateTagId (CategoryDTO categories, IngredientDTO ingredient)
    {
          return new TagDTO
          {
              Category_id = categories.Id,
              Ingredient_id = ingredient.Id
          };
    }

    // Get all stock images that match tags
    //public async Task<TagDTO> GetImagesFromTags(string tags)
    //{
        
    //}
    
}