using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Account;
using FoodplannerModels.Tags;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class CategoryController(CategoryService categoryService) : BaseController
{
    // URL : "api/Tag/CreateCategory" 
    // Creates a new category from category DTO
    // When success returns 201, when failed returns 400
    [HttpPost]
    [Authorize(Roles = "Parent, Teacher")]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categories)
    {
        var category = await categoryService.CreateCategory(categories);
        if (category.Id > 0)
        {
            return Created(string.Empty, category.Id);
        }
        return BadRequest();
    }
}