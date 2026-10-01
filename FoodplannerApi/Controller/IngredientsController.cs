using FoodplannerModels.Account;
using FoodplannerModels.Lunchbox;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FoodplannerModels.Auth;

// Adds the IngredientsController to the FoodPlannerApi.controller namespace
namespace FoodplannerApi.Controller;

public class IngredientsController(IIngredientService ingredientService, IAuthService authService) : BaseController
{

    // URL: api/Ingredients/GetAll
    // Retrieves all ingredients. Returns 200 OK with the list of ingredients.
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<IngredientDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var ingredients = await ingredientService.GetAllIngredientsAsync();
        return Ok(ingredients);
    }

    // URL: api/Ingredients/GetAllByUser
    // Retrieves all ingredients associated with a user ID extracted from the JWT token in the Authorization header.
    [HttpGet]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IEnumerable<IngredientDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllByUser([FromHeader(Name = "Authorization")] string token)
    {
        try
        {

            // Retrieves user id from JWT token in the Authorization header.
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Calls the service to fetch all ingredients
            var ingredients = await ingredientService.GetAllIngredientsByUserAsync(id);
            return Ok(ingredients);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Ingredients/Get/{id}
    // Retrieves an ingredient by its ID.
    [HttpGet("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IngredientDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var ingredient = await ingredientService.GetIngredientByIdAsync(id);
        if (ingredient == null)
        {
            return NotFound();
        }
        return Ok(ingredient);
    }

    // URL: api/Ingredients/Create
    // Creates a new ingredient associated with the user ID extracted from the JWT token in the Authorization header.
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IngredientDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] IngredientDTO ingredient)
    {
        // Retrieves user id from JWT token in the Authorization header.
        var idString = authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
        }

        // Calls the service to create a new ingredient associated with the user ID
        var result = await ingredientService.CreateIngredientAsync(ingredient, id);
        if (result > 0)
        {
            var createdIngredient = await ingredientService.GetIngredientByIdAsync(result);
            return CreatedAtAction(nameof(Get), new { id = result }, createdIngredient);
        }
        return BadRequest();
    }

    // URL: api/Ingredients/Update/{id}
    // Updates an existing ingredient by its ID using provided IngredientDTO.
    [HttpPut("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IngredientDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] IngredientDTO ingredientDto, int id)
    {
        var result = await ingredientService.UpdateIngredientAsync(ingredientDto, id);
        if (result > 0)
        {
            var changedIngredient = await ingredientService.GetIngredientByIdAsync(id);
            return Ok(changedIngredient);
        }
        return BadRequest();
    }

    // URL: api/Ingredients/Delete/{id}
    // Deletes an ingredient by its ID, returns deleted ingredient if successful.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IngredientDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var ingredient = await ingredientService.GetIngredientByIdAsync(id);
        var result = await ingredientService.DeleteIngredientAsync(id);
        if (result > 0)
        {
            return Ok(ingredient);
        }
        return NotFound();
    }
}