using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerModels.Image;
using FoodplannerModels.Tags;
using FoodplannerServices.Image;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



// Adds the TagController to the FoodPlannerApi.controller namespace
namespace FoodplannerApi.Controller;

public class TagController(ITagService tagService, IFoodImageService foodImageService) : BaseController
{
    // URL : "api/Tag/GetImagesFromTags"
    [HttpGet]
    [Authorize(Roles = "Child, Parent, Teacher")]
    [ProducesResponseType(typeof(TagDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImagesFromTags([FromBody] string tags)
    {
    {
        try
        {
            
            if (string.IsNullOrEmpty(tags))
            {
                var images =  foodImageService.GetAllStockImages();
                return Ok(images);
            } else
            {
                var images = await tagService.GetImagesFromTags(tags);
                return Ok(images);
            }
                
        }
        catch (Exception)
        {
            return BadRequest(new { Message = "Billeder kunne ikke blive fundet" });
        }
    }
    }

}