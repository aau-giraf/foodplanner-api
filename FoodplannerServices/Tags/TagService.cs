using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;
using FoodplannerServices.Image;
using FoodplannerDataAccessSql.Tags;

// Adds TagService to the class to the FoodplannerServices.Tags namespace
namespace FoodplannerServices.Tags;

// Service implementation for Tag business logic
public class TagService(ITagRepository tagRepository, IFoodImageService foodImageService) : ITagService
{
    // Method for creating a Tag
    // Returns a new Tag with ID
    public async Task<TagDTO> CreateTag (CategoryDTO category, IngredientDTO ingredient)
    {
          return new TagDTO
          {
              Category_id = category.Id,
              Ingredient_id = ingredient.Id
          };
    }

    // Get all stock images that match tags (e.g "fruit" AND "sliced")
    public async Task<IEnumerable<FoodImageDTO>> GetImagesFromTags(string tags)
    {
        // split tags string into a list  
        var tagsList = tags
            .Split(',', StringSplitOptions.RemoveEmptyEntries) // comma separated
            .Select(t => t.Trim()) 
            .Where(t => !string.IsNullOrWhiteSpace(t)) //remove whitespace 
            .Distinct(StringComparer.OrdinalIgnoreCase) //ignore duplicates
            .ToList();
        
        // Retrieves list of strings of image ids, where ingredient matches all tags in tagslist 
        var imageIds = await tagRepository.GetImageIdsFromTags(tagsList);

        // Retrieves FoodImageDTO for each of the image ids 
        List<FoodImageDTO> images = new List<FoodImageDTO>(); 
        foreach (var id in imageIds)
        {
            var image = await foodImageService.GetFoodImage(id);
            images.Add(image);
        }

        return images.AsEnumerable();
        
    }
    
}