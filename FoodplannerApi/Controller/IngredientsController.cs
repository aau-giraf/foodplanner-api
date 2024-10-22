using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

/**
* The IngredientsController class handles CRUD (Create, Read, Update, Delete) operations for the Ingredient entity.
* It uses the IngredientService to interact with the database and process ingredient-related requests.
*/
public class IngredientsController (IngredientService ingredientService) : BaseController
{
    // Private field to hold the injected IngredientService.
    private readonly IngredientService _ingredientService = ingredientService;

    // Get all ingredients
    [HttpGet]
    public async Task<IActionResult> GetAll(){
        // Calls the service to fetch all ingredients
        var ingredients = await _ingredientService.GetAllIngredientsAsync(); // Fetch all ingredients.
        return Ok(ingredients); // Returns the list of ingredients with a 200 OK status
    }
    // Get a specific ingredient by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id){
        var ingredient = await _ingredientService.GetIngredientByIdAsync(id); / Fetch the ingredient by ID.
        if (User == null){  // Check if the user exists
            return NotFound(); // Returns 404 if not found
        }
        return Ok(ingredient); // Returns the found ingredient with a 200 OK status
    }

    // Create a new ingredient
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Ingredient ingredient){
        // Calls the service to fetch the ingredient by ID
        var result = await _ingredientService.CreateIngredientAsync(ingredient);
        if (result > 0){
            // Returns 201 Created with the new ingredient's location
            return CreatedAtAction(nameof(Get), new { id = ingredient.Id}, ingredient);
        }
        return BadRequest(); // Returns 400 if the creation fails
    }

    // Update an existing ingredient
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Ingredient ingredient, int id){
        var result = await _ingredientService.UpdateIngredientAsync(ingredient, id); // Calls the service to update the ingredient by ID
        if (result > 0){
            var changedIngredient = await _ingredientService.GetIngredientByIdAsync(id); // Fetch updated ingredient
            return Ok(changedIngredient); // Returns the updated ingredient with a 200 OK status
        }
        return BadRequest(); // Returns 400 if the update fails
    }

    // Delete an ingredient by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
         // Calls the service to delete the ingredient by ID
        var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
        var result = await _ingredientService.DeleteIngredientAsync(id);
        if (result > 0){
            return Ok(ingredient);
        }
        return NotFound(); // Returns 404 if the ingredient was not found
    }
}