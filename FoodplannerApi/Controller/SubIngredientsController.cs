using FoodplannerModels.Account;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Auth;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class SubIngredientsController(SubIngredientService subIngredientService, IAuthService authService) : BaseController
{
    // URL: api/SubIngredients/GetByParentId/{id}
    // Retrieves all subingredients associated with a specific parent ingredient ID
    [HttpGet("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByParentId(int id)
    {
        var subingredient = await subIngredientService.GetAllSubIngredientsByParentIdAsync(id);
        if (subingredient == null)
        {
            return NotFound();
        }
        return Ok(subingredient);
    }

    // URL: api/SubIngredients/Create
    // Creates a new subingredient from SubIngredient under user associated with authorization token
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] SubIngredient subingredient)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponseDTO { Message = ["Invalid user ID"] });
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
            return BadRequest(new ErrorResponseDTO { Message = [e.Message] });
        }
    }

    // URL: api/SubIngredients/Update/{parent_id}/{child_id}
    // Updates an existing subingredient by its parent and child id
    [HttpPut("{parent_id}/{child_id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] SubIngredient newSubIngredient, int parent_id, int child_id)
    {
        var oldSubIngredient = new SubIngredient { Parent_id = parent_id, Child_id = child_id, order_no = 0};
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
    [ProducesResponseType(typeof(SubIngredient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromBody] SubIngredient subIngredient)
    {
        var result = await subIngredientService.DeleteSubIngredientAsync(subIngredient);
        if (result > 0)
        {
            return Ok(result);
        }
        return NotFound();
    }
}