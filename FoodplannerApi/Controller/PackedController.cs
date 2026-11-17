using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Adds the PackedIngredientController to the FoodPlannerApi.Controller namespace
namespace FoodplannerApi.Controller;

public class PackedController(IPackedService packedIngredientService) : BaseController
{
    // URL: api/Packed/Create
    // Creates new packed ingredient from PackedDTO
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] Packed packIngredient)
    {
        var result = await packedIngredientService.CreatePackedAsync(packIngredient);
        if (result > 0)
        {
            return StatusCode(StatusCodes.Status201Created, packIngredient);
        }
        return BadRequest();
    }

    // URL: api/Packed/Update/{meal_id}/{ingredient_id}
    // Updates the order no. of ingredients in a packed
    [HttpPut("{meal_id}/{ingredient_id}")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(Packed), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] Packed newPackedDTO, int ingredient_id, int meal_id)
    {
        var oldPackedDTO = new Packed{Ingredient_id = ingredient_id, Meal_id = meal_id, order_no = 0};
        var result = await packedIngredientService.UpdatePackedAsync(oldPackedDTO, newPackedDTO);
        if (result > 0)
        {
            return Ok(result);
        }
        return BadRequest();
    }

    // URL: api/Packed/Delete
    // Deletes a packed ingredient by its ID
    [HttpDelete]
    [Authorize(Roles = "Parent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromBody] Packed packedDTO)
    {
        var result = await packedIngredientService.DeletePackedAsync(packedDTO);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }
}