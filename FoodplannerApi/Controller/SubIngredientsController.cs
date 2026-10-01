using FoodplannerModels.Account;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Adds the SubIngredientsController to the FoodPlannerApi.Controller namespace
namespace FoodplannerApi.Controller;

public class SubIngredientsController(ISubIngredientService subIngredientService, IAuthService authService) : BaseController
{

    // URL: api/SubIngredients/GetAll
    // Retrieves all subingredients
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredient>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var subingredients = await subIngredientService.GetAllSubIngredientsAsync();
        return Ok(subingredients);
    }

    // URL: api/SubIngredients/GetAllByUser
    // Retrieves all subingredients associated with authorization token
    [HttpGet]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredient>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllByUser([FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Retrieve all subingredients associated with the user ID
            var subingredients = await subIngredientService.GetAllSubIngredientsByUserAsync(userId);
            return Ok(subingredients);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/SubIngredients/Get/{id}
    // Retrieves a specific subingredient by its ID
    [HttpGet("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var subingredient = await subIngredientService.GetSubIngredientByIdAsync(id);
        if (subingredient == null)
        {
            return NotFound();
        }
        return Ok(subingredient);
    }

    // URL: api/SubIngredients/Create
    // Creates a new subingredient from SubIngredientDTO under user associated with authorization token
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] SubIngredientDTO subingredient)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Calls the service to create a new subingredient under the user ID
            var result = await subIngredientService.CreateSubIngredientAsync(subingredient, userId);
            if (result > 0)
            {
                var createdSubIngredient = await subIngredientService.GetSubIngredientByIdAsync(result);
                return CreatedAtAction(nameof(Get), new { id = result }, createdSubIngredient);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/SubIngredients/Update/{id}
    // Updates an existing subingredient by its ID and SubIngredient
    [HttpPut("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] SubIngredient subingredient, int id)
    {
        var result = await subIngredientService.UpdateSubIngredientAsync(subingredient, id);
        if (result > 0)
        {
            var changedSubIngredient = await subIngredientService.GetSubIngredientByIdAsync(id);
            return Ok(changedSubIngredient);
        }
        return BadRequest();
    }

    // URL: api/SubIngredients/Delete/{id}
    // Delete a subingredient by ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var subingredient = await subIngredientService.GetSubIngredientByIdAsync(id);
        var result = await subIngredientService.DeleteSubIngredientAsync(id);
        if (result > 0)
        {
            return Ok(subingredient);
        }
        return NotFound();
    }
}