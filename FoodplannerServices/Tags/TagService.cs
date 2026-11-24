using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;
using FoodplannerServices.Image;
using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Image;

// Adds TagService to the class to the FoodplannerServices.Tags namespace
namespace FoodplannerServices.Tags;

// Service implementation for Tag business logic
public class TagService(ITagRepository tagRepository, IFoodImageService foodImageService) : ITagService
{
    // Method for creating a Tag
    // Returns a new Tag with ID
    public async Task<int> CreateTagAsync (int category_id, int ingredient_id)
    {
          var tag = new Tag
          {
              Category_id = category_id,
              Ingredient_id = ingredient_id
          };

            // Checks if tag already exists
          if (await tagRepository.TagExistsAsync(tag.Category_id, tag.Ingredient_id))
        {
            throw new InvalidOperationException("Tag eksisterer allerede");
        }

        // Inserts tag into the database
        return await tagRepository.InsertAsync(tag);
    }

    // Get all stock images that match tags (e.g "fruit" AND "sliced")
    public async Task<IEnumerable<FoodImage>> GetImagesFromTagsAsync(string tags)
    {
        // split tags string into a list  
        var tagsList = tags
            .Split(',', StringSplitOptions.RemoveEmptyEntries) // comma separated
            .Select(t => t.Trim()) 
            .Where(t => !string.IsNullOrWhiteSpace(t)) //remove whitespace 
            .Distinct(StringComparer.OrdinalIgnoreCase) //ignore duplicates
            .ToList();
        
        // Retrieves list of strings of image ids, where ingredient matches all tags in tagslist 
        var imageIds = await tagRepository.GetImageIdsFromTagsAsync(tagsList);

        // Retrieves FoodImage for each of the image ids 
        List<FoodImage> images = new List<FoodImage>(); 
        foreach (var id in imageIds)
        {
            var image = await foodImageService.GetFoodImage(id);
            images.Add(image);
        }

        return images.AsEnumerable();
        
    }
    
}