
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller; 

public class PixabayController(IPixabayService pixabayService) : BaseController
{

    // URL: api/Pixabay/Get 
    [ProducesResponseType(typeof(IEnumerable<PixaBayDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    public async Task<IActionResult> Get()
    {
        var image = await pixabayService.Get(); 
        if (image == null)
        {
            return NotFound();
        }
        return Ok(); 
    }
    
}