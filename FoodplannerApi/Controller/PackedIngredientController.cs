using FoodplannerModels.Lunchbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Adds the PackedIngredientController to the FoodPlannerApi.Controller namespace
namespace FoodplannerApi.Controller;

public class PackedIngredientController(IPackedIngredientService packedIngredientService) : BaseController
{
    
    // URL: api/PackedIngredient/GetAll
    // Retrieves all packed ingredients
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<PackedIngredientDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var packedIngredients = await packedIngredientService.GetAllPackedIngredientsAsync();
        return Ok(packedIngredients);
    }

    // URL: api/PackedIngredient/Get/{id}
    // Retrieves a packed ingredient by its ID
    [HttpGet("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(PackedIngredientDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var packedIngredient = await packedIngredientService.GetPackedIngredientByIdAsync(id);
        if (packedIngredient == null)
        {
            return NotFound();
        }
        return Ok(packedIngredient);
    }

    // URL: api/PackedIngredient/Create
    // Creates new packed ingredient from PackedIngredientProperDTO
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PackedIngredientProperDTO packIngredient)
    {
        var result = await packedIngredientService.CreatePackedIngredientAsync(packIngredient);
        if (result > 0)
        {
            var createdPI = await packedIngredientService.GetPackedIngredientByIdAsync(result);
            return CreatedAtAction(nameof(Get), new { id = result }, createdPI);
        }
        return BadRequest();
    }

    // URL: api/PackedIngredient/Update/{id}
    // Updates an existing packed ingredient by its ID and PackedIngredientDTO
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(PackedIngredientDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] PackedIngredientDTO packedIngredientDto, int id)
    {
        var result = await packedIngredientService.UpdatePackedIngredientAsync(packedIngredientDto, id);
        if (result > 0)
        {
            var changedPackedIngredient = await packedIngredientService.GetPackedIngredientByIdAsync(id);
            return Ok(changedPackedIngredient);
        }
        return BadRequest();
    }

    // URL: api/PackedIngredient/Delete/{id}
    // Deletes a packed ingredient by its ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Parent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await packedIngredientService.DeletePackedIngredientAsync(id);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }

    // URL: api/PackedIngredient/UpdateOrder
    // Updates the order of packed ingredients based on a list of PackedIngredientDTOs
    [HttpPut]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateOrder([FromBody] List<PackedIngredientDTO> packedIngredientsDto)
    {
        var result = await packedIngredientService.UpdatePackedIngredientOrderAsync(packedIngredientsDto);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }
}