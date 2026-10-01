using FoodplannerModels.Lunchbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Adds the SubIngredientRelationController to the FoodPlannerApi.Controller namespace
namespace FoodplannerApi.Controller;

public class SubIngredientRelationController(ISubIngredientRelationService subIngredientRelationService) : BaseController
{

    // URL: api/SubIngredientRelation/GetAll
    // Retrieves all subingredient relations
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredientRelationProperDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var relations = await subIngredientRelationService.GetAllSubIngredientRelationsAsync();
        return Ok(relations);
    }

    // URL: api/SubIngredientRelation/GetByIngredientId/{ingredientId}
    // Retrieves all subingredient relations for a specific ingredient by its ID
    [HttpGet("{ingredientId}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredientRelationDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIngredientId(int ingredientId)
    {
        var relations = await subIngredientRelationService.GetAllSubIngredientRelationsByIngredientIdAsync(ingredientId);
        return Ok(relations);
    }

    // URL: api/SubIngredientRelation/Get/relation/{id}
    // Retrieves a specific subingredient relation by its ID
    [HttpGet("relation/{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(SubIngredientRelation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var relation = await subIngredientRelationService.GetSubIngredientRelationByIdAsync(id);
        if (relation == null)
        {
            return NotFound();
        }
        return Ok(relation);
    }

    // URL: api/SubIngredientRelation/Create
    // Creates a new subingredient relation from SubIngredientRelationProperDTO
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredientRelation), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] SubIngredientRelationProperDTO relation)
    {
        // Calls the service to create a new subingredient relation
        var result = await subIngredientRelationService.CreateSubIngredientRelationAsync(
            relation.Ingredient_id, 
            relation.Subingredient_id);
            
        // Verifies if the creation was successful and returns the appropriate response
        if (result > 0)
        {
            var createdRelation = await subIngredientRelationService.GetSubIngredientRelationByIdAsync(result);
            return CreatedAtAction(nameof(Get), new { id = result }, createdRelation);
        }
        return BadRequest();
    }

    // URL: api/SubIngredientRelation/Update/{id}
    // Updates an existing subingredient relation by its ID and SubIngredientRelation
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(SubIngredientRelation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] SubIngredientRelation relation, int id)
    {
        var result = await subIngredientRelationService.UpdateSubIngredientRelationAsync(relation, id);
        if (result > 0)
        {
            var changedRelation = await subIngredientRelationService.GetSubIngredientRelationByIdAsync(id);
            return Ok(changedRelation);
        }
        return BadRequest();
    }

    // URL: api/SubIngredientRelation/Delete/{id}
    // Deletes a subingredient relation by its ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Parent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await subIngredientRelationService.DeleteSubIngredientRelationAsync(id);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }

    // URL: api/SubIngredientRelation/UpdateOrder
    // Updates the order of subingredient relations based on a list of SubIngredientRelation
    [HttpPut]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateOrder([FromBody] List<SubIngredientRelation> relations)
    {
        var result = await subIngredientRelationService.UpdateSubIngredientRelationOrderAsync(relations);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }
}