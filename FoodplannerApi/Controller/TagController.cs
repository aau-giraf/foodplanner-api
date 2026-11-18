using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerModels.Image;
using FoodplannerModels.Lunchbox;
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
     // Creates a new tag from category DTO and ingredient DTO
    // When success returns 201, when failed returns 400
    [HttpPost]
    [Authorize(Roles = "Parent, Teacher")]
    [ProducesResponseType(typeof(TagDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTag([FromBody] CategoryDTO categories, [FromBody] IngredientDTO ingredient)
    {
         var result = await tagService.createTagId(categories, ingredient);
        if (result.Category_id > 0)
        {
            return Ok(new {Message = "Tag ID oprettet"});
        }
        return BadRequest(new ErrorResponse { Message = new[] { "Tag ID kunne ikke oprettes" } });
    }

    // URL : "api/Tag/GetImagesFromTags"
    // Get all images that has all the tags (e.g. "fruit" AND "sliced"), 
    [HttpGet]
    [Authorize(Roles = "Child, Parent, Teacher")]
    [ProducesResponseType(typeof(TagDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImagesFromTags([FromBody] string tags)
    {
        if (string.IsNullOrEmpty(tags))
        {
            var images = await foodImageService.GetAllStockImages();
            return Ok(images);
        } else
        {
            var images = await tagService.GetImagesFromTags(tags);
            return Ok(images);
        }
    
    }

}
