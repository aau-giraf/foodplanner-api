using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Account;
using FoodplannerModels.Tags;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class CategoryController(CategoriesService categoriesService) : BaseController
{
    // Creates a new category from categories DTO
    // When success returns 201, when failed returns 400
    [HttpPost]
    [ProducesResponseType(typeof(Categories), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] Categories categories)
    {
    
        var id = await categoriesService.CreateCategory(categories);
        if (id > 0)
        {
            return Created(string.Empty, id);
        }
        return BadRequest();


    }
}