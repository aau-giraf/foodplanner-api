using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Adds the PackedIngredientController to the FoodPlannerApi.Controller namespace
namespace FoodplannerApi.Controller;

public class PackedIngredientController(IPackedService packedIngredientService) : BaseController
{
    
    // URL: api/PackedIngredient/GetAll
    // Retrieves all packed ingredients
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<PackedDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var packedIngredients = await packedIngredientService.GetAllPackedAsync();
        return Ok(packedIngredients);
    }

    // URL: api/PackedIngredient/Get/{id}
    // Retrieves all packed ingredients associated with a specific meal ID
    [HttpGet("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<PackedDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var packedIngredient = await packedIngredientService.GetAllPackedByMealIdAsync(id);
        if (packedIngredient == null)
        {
            return NotFound();
        }
        return Ok(packedIngredient);
    }

    // URL: api/PackedIngredient/Create
    // Creates new packed ingredient from PackedDTO
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PackedDTO packIngredient)
    {
        var result = await packedIngredientService.CreatePackedAsync(packIngredient);
        if (result > 0)
        {
            return CreatedAtAction(nameof(Get), new { id = result }, packIngredient);
        }
        return BadRequest();
    }

    // URL: api/PackedIngredient/Update/{id}
    // Updates an existing packed ingredient by its ID and PackedIngredientDTO
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(PackedDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] PackedDTO oldPackedDTO, [FromBody] PackedDTO newPackedDTO)
    {
        var result = await packedIngredientService.UpdatePackedAsync(oldPackedDTO, newPackedDTO);
        if (result > 0)
        {
            return Ok(result);
        }
        return BadRequest();
    }

    // URL: api/PackedIngredient/Delete
    // Deletes a packed ingredient by its ID
    [HttpDelete]
    [Authorize(Roles = "Parent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromBody] PackedDTO packedDTO)
    {
        var result = await packedIngredientService.DeletePackedAsync(packedDTO);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }
}