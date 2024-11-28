using FoodplannerModels.Account;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FoodplannerApi.Controller;

/**
* The IngredientsController class handles CRUD (Create, Read, Update, Delete) operations for the Ingredient entity.
* It uses the IngredientService to interact with the database and process ingredient-related requests.
*/
public class IngredientsController(IngredientService ingredientService, AuthService authService) : BaseController
{
    // Private field to hold the injected IngredientService.
    private readonly IngredientService _ingredientService = ingredientService;
    private readonly AuthService _authService = authService;

    // Get all ingredients
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> GetAll()
    {
        // Calls the service to fetch all ingredients
        var ingredients = await _ingredientService.GetAllIngredientsAsync(); // Fetch all ingredients.
        return Ok(ingredients); // Returns the list of ingredients with a 200 OK status
    }

    // Get all ingredients by user
    [HttpGet]
    [Authorize(Roles = "Child, Parent")]
    public async Task<IActionResult> GetAllByUser([FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
            }
            // Calls the service to fetch all ingredients
            var ingredients = await _ingredientService.GetAllIngredientsByUserAsync(id); // Fetch all ingredients by user.
            return Ok(ingredients); // Returns the list of ingredients with a 200 OK status
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // Get a specific ingredient by ID
    [HttpGet("{id}")]
    [Authorize(Roles = "Child, Parent")]
    public async Task<IActionResult> Get(int id)
    {
        var ingredient = await _ingredientService.GetIngredientByIdAsync(id); // Fetch the ingredient by ID.
        if (ingredient == null)
        {  // Check if the ingredient exists
            return NotFound(); // Returns 404 if not found
        }
        return Ok(ingredient); // Returns the found ingredient with a 200 OK status
    }

    // Create a new ingredient
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] IngredientDTO ingredient)
    {
        var idString = _authService.RetrieveIdFromJwtToken(token); // Use the method to get the parentId from the token
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
        }
        // Calls the service to fetch the ingredient by ID
        var result = await _ingredientService.CreateIngredientAsync(ingredient, id);
        if (result > 0)
        {
            var createdIngredient = await _ingredientService.GetIngredientByIdAsync(result);
            return CreatedAtAction(nameof(Get), new { id = result }, createdIngredient);
        }
    }

    // Update an existing ingredient
    [HttpPut("{id}")]
    [Authorize(Roles = "Child, Parent")]
    public async Task<IActionResult> Update([FromBody] Ingredient ingredient, int id)
    {
        var result = await _ingredientService.UpdateIngredientAsync(ingredient, id); // Calls the service to update the ingredient by ID
        if (result > 0)
        {
            var changedIngredient = await _ingredientService.GetIngredientByIdAsync(id); // Fetch updated ingredient
            return Ok(changedIngredient); // Returns the updated ingredient with a 200 OK status
        }
        return BadRequest(); // Returns 400 if the update fails
    }

    // Delete an ingredient by ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Child, Parent")]
    public async Task<IActionResult> Delete(int id)
    {
        // Calls the service to delete the ingredient by ID
        var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
        var result = await _ingredientService.DeleteIngredientAsync(id);
        if (result > 0)
        {
            return Ok(ingredient);
        }
        return NotFound(); // Returns 404 if the ingredient was not found
    }

    public class IngredientContainer
    {
        public string Name { get; set; }
        public int? Image_ref { get; set; }
    }
}