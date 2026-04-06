using FoodplannerModels.Account;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

/// <summary>
/// Controller for managing subingredients
/// </summary>
public class SubIngredientsController : BaseController
{
    private readonly ISubIngredientService _subIngredientService;
    private readonly IAuthService _authService;

    public SubIngredientsController(ISubIngredientService subIngredientService, IAuthService authService)
    {
        _subIngredientService = subIngredientService;
        _authService = authService;
    }

    /// <summary>
    /// Get all subingredients (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredient>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var subingredients = await _subIngredientService.GetAllSubIngredientsAsync();
        return Ok(subingredients);
    }

    /// <summary>
    /// Get all subingredients for the logged-in user
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredient>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllByUser([FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
            }

            var subingredients = await _subIngredientService.GetAllSubIngredientsByUserAsync(userId);
            return Ok(subingredients);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    /// <summary>
    /// Get a specific subingredient by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var subingredient = await _subIngredientService.GetSubIngredientByIdAsync(id);
        if (subingredient == null)
        {
            return NotFound();
        }
        return Ok(subingredient);
    }

    /// <summary>
    /// Create a new subingredient
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] SubIngredientDTO subingredient)
    {
        try
        {
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
            }

            var result = await _subIngredientService.CreateSubIngredientAsync(subingredient, userId);
            if (result > 0)
            {
                var createdSubIngredient = await _subIngredientService.GetSubIngredientByIdAsync(result);
                return CreatedAtAction(nameof(Get), new { id = result }, createdSubIngredient);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    /// <summary>
    /// Update an existing subingredient
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] SubIngredient subingredient, int id)
    {
        var result = await _subIngredientService.UpdateSubIngredientAsync(subingredient, id);
        if (result > 0)
        {
            var changedSubIngredient = await _subIngredientService.GetSubIngredientByIdAsync(id);
            return Ok(changedSubIngredient);
        }
        return BadRequest();
    }

    /// <summary>
    /// Delete a subingredient by ID
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var subingredient = await _subIngredientService.GetSubIngredientByIdAsync(id);
        var result = await _subIngredientService.DeleteSubIngredientAsync(id);
        if (result > 0)
        {
            return Ok(subingredient);
        }
        return NotFound();
    }
}