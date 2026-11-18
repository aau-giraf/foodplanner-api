using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Account;
using FoodplannerModels.Tags;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class CategoryController(CategoriesService categoriesService) : BaseController
{
    // Creates a new category from category DTO
    // When success returns 201, when failed returns 400
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CategoryDTO category)
    {
    
        var id = await categoriesService.CreateCategory(category);
        if (id > 0)
        {
            return Created(string.Empty, id);
        }
        return BadRequest();


    }
}