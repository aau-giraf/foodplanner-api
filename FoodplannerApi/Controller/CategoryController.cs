using FoodplannerModels.Account;
using FoodplannerModels.Tags;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class CategoryController(CategoriesService categoriesService) : BaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CategoryDTO category)
    {

        return BadRequest(new ErrorResponse {Message = [""]});
    }
}