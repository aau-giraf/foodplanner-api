using FoodplannerModels.Account;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Auth;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Adds the SubIngredientsController to the FoodPlannerApi.Controller namespace
namespace FoodplannerApi.Controller;

public class SubIngredientsController(SubIngredientService subIngredientService, IAuthService authService) : BaseController
{

    // URL: api/SubIngredients/GetAllSubIngredients
    // Retrieves all subingredients
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredientDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var subingredients = await subIngredientService.GetAllSubIngredientsAsync();
        return Ok(subingredients);
    }

    // URL: api/SubIngredients/GetAllByUser
    // Retrieves all subingredients associated with authorization token
    [HttpGet]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IEnumerable<SubIngredientDTO>), StatusCodes.Status200OK)]
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
            var subingredients = await subIngredientService.GetAllSubIngredientsByParentIdAsync(userId);
            return Ok(subingredients);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/SubIngredients/Get/{id}
    // Retrieves all subingredients associated with a specific parent ingredient ID
    [HttpGet("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredientDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var subingredient = await subIngredientService.GetAllSubIngredientsByParentIdAsync(id);
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
    [ProducesResponseType(typeof(SubIngredientDTO), StatusCodes.Status200OK)]
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
            var result = await subIngredientService.CreateSubIngredientAsync(subingredient);
            if (result > 0)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/SubIngredients/Update/{parent_id}/{child_id}
    // Updates an existing subingredient by its parent and child id
    [HttpPut("{parent_id}/{child_id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredientDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] SubIngredientDTO newSubIngredient, int parent_id, int child_id)
    {
        var oldSubIngredient = new SubIngredientDTO { Parent_id = parent_id, Child_id = child_id, order_no = 0};
        var result = await subIngredientService.UpdateSubIngredientAsync(oldSubIngredient, newSubIngredient);
        if (result > 0)
        {
            return Ok(result);
        }
        return BadRequest();
    }

    // URL: api/SubIngredients/Delete
    // Delete a subingredient
    [HttpDelete]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredientDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromBody] SubIngredientDTO subingredientDTO)
    {
        var result = await subIngredientService.DeleteSubIngredientAsync(subingredientDTO);
        if (result > 0)
        {
            return Ok(result);
        }
        return NotFound();
    }
}