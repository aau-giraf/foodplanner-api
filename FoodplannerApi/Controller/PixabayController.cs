
using FoodplannerModels.Account;
using FoodplannerModels.Image;
using FoodplannerServices.Image;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller; 

public class PixabayController(IPixabayService pixabayService) : BaseController
{

    // URL: api/Pixabay/Get
    [HttpPost]
    [Authorize(Roles = "Child, Parent, Teacher")]
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
    
}