using FoodplannerModels.Lunchbox;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

/// <summary>
/// Controller for managing relationships between ingredients and subingredients
/// </summary>
public class SubIngredientRelationController : BaseController
{
    private readonly ISubIngredientRelationService _subIngredientRelationService;

    public SubIngredientRelationController(ISubIngredientRelationService subIngredientRelationService)
    {
        _subIngredientRelationService = subIngredientRelationService;
    }

    /// <summary>
    /// Get all subingredient relations (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredientRelationProperDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var relations = await _subIngredientRelationService.GetAllSubIngredientRelationsAsync();
        return Ok(relations);
    }

    /// <summary>
    /// Get all subingredient relations for a specific ingredient
    /// </summary>
    [HttpGet("{ingredientId}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredientRelationDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIngredientId(int ingredientId)
    {
        var relations = await _subIngredientRelationService.GetAllSubIngredientRelationsByIngredientIdAsync(ingredientId);
        return Ok(relations);
    }

    /// <summary>
    /// Get a specific subingredient relation by ID
    /// </summary>
    [HttpGet("relation/{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(SubIngredientRelation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var relation = await _subIngredientRelationService.GetSubIngredientRelationByIdAsync(id);
        if (relation == null)
        {
            return NotFound();
        }
        return Ok(relation);
    }

    /// <summary>
    /// Create a new subingredient relation
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredientRelation), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] SubIngredientRelationProperDTO relation)
    {
        var result = await _subIngredientRelationService.CreateSubIngredientRelationAsync(
            relation.Ingredient_id, 
            relation.Subingredient_id);
            
        if (result > 0)
        {
            var createdRelation = await _subIngredientRelationService.GetSubIngredientRelationByIdAsync(result);
            return CreatedAtAction(nameof(Get), new { id = result }, createdRelation);
        }
        return BadRequest();
    }

    /// <summary>
    /// Update an existing subingredient relation
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(SubIngredientRelation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] SubIngredientRelation relation, int id)
    {
        var result = await _subIngredientRelationService.UpdateSubIngredientRelationAsync(relation, id);
        if (result > 0)
        {
            var changedRelation = await _subIngredientRelationService.GetSubIngredientRelationByIdAsync(id);
            return Ok(changedRelation);
        }
        return BadRequest();
    }

    /// <summary>
    /// Delete a subingredient relation by ID
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Parent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _subIngredientRelationService.DeleteSubIngredientRelationAsync(id);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }

    /// <summary>
    /// Update the order of subingredients within an ingredient
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateOrder([FromBody] List<SubIngredientRelation> relations)
    {
        var result = await _subIngredientRelationService.UpdateSubIngredientRelationOrderAsync(relations);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }
}