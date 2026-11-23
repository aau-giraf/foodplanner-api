using FoodplannerModels.Account;
using FoodplannerModels.Tags;
using FoodplannerServices.Image;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



// Adds the TagController to the FoodPlannerApi.controller namespace
namespace FoodplannerApi.Controller;

public class TagController(ITagService tagService, IFoodImageService foodImageService) : BaseController
{
    // URL : "api/Tag/CreateTag"
     // Creates a new tag from category ID and ingredient ID
    // When success returns 201, when failed returns 400
    [HttpPost]
    [Authorize(Roles = "Parent, Teacher")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTag([FromBody] TagDTO tagDTO)
    {
        // Validate the model state to ensure the incoming data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var id = await tagService.CreateTagAsync(tagDTO.Category_id, tagDTO.Ingredient_id);
        if (id > 0)
        {
            return Created(string.Empty, id);
        }
        return BadRequest();
    }

    // URL : "api/Tag/GetImagesFromTags"
    // Get all images that has all the tags (e.g. "fruit" AND "sliced"), 
    [HttpGet]
    [Authorize(Roles = "Child, Parent, Teacher")]
    [ProducesResponseType(typeof(TagDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImagesFromTags([FromQuery] string? tags = null)
    {
        if (string.IsNullOrEmpty(tags))
        {
            var images = await foodImageService.GetAllStockImagesAsync();
            return Ok(images);
        } else
        {
            var images = await tagService.GetImagesFromTagsAsync(tags);
            return Ok(images);
        }
    
    }

}
