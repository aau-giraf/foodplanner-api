using FoodplannerModels.Tags;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class CategoryController(ICategoryService categoryService) : BaseController
{
    // URL : "api/Category/CreateCategory" 
    // Creates a new category from CreateCategoryDTO
    // When success returns 201, when failed returns 400
    [HttpPost]
    [Authorize(Roles = "Parent, Teacher")]
    [ProducesResponseType(typeof(CreateCategoryDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO createCategory)
    {
        // Validate the model state to ensure the incoming data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var id = await categoryService.CreateCategoryAsync(createCategory);
        if (id > 0)
        {
            return Created(string.Empty, id);
        }
        return BadRequest();
    }
}