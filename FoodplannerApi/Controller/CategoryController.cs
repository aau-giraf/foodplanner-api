using FoodplannerModels.Account;
using FoodplannerModels.Tags;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class CategoryController(CategoriesService categoriesService) : BaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(Categories), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] Categories categories)
    {

        return BadRequest(new ErrorResponse {Message = [""]});
    }
}