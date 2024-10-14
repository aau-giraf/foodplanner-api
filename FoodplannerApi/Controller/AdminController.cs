using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

[Authorize(Policy = "AdminPolicy")]
public class AdminController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAdminTest()
    {
        
        return Ok();
    }
}