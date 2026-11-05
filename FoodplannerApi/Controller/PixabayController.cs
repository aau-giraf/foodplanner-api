
using FoodplannerModels.Account;
using FoodplannerModels.Image;
using FoodplannerServices.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodplannerServices.Auth;
using FoodplannerModels.Auth;

namespace FoodplannerApi.Controller; 

public class PixabayController(IPixabayService pixabayService, IAuthService authService) : BaseController
{

    // URL: api/Pixabay/Get
    [HttpPost]
    [Authorize(Roles = "Parent, Teacher, Admin")]
    [ProducesResponseType(typeof(IEnumerable<Pixabay>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromBody] PixabayDTO imageSearch)
    {
        var image = await pixabayService.GetPixabayImage(imageSearch); 
        if (image == null)
        {
            return NotFound();
        }
        return Ok(image);
    }

    [HttpPost]
    [Authorize(Roles = "Parent, Teacher, Admin")]
    [ProducesResponseType(typeof(IEnumerable<Pixabay>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SavePixabayUrl([FromHeader(Name = "Authorization")] string token, [FromBody] string keyword, string url)
    {
        try{
            // Retrieve UserId from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Call the service to save the image URL
            var imageId = await pixabayService.SaveImageUrl(userId, keyword, url);
            return Ok(imageId);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse {Message = [e.Message]});
        }
    }
    
}