using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

/**
* The controller for the Ingredient class.
*/
public class IngredientsController (IngredientService ingredientService) : BaseController
{
    private readonly IngredientService _ingredientService = ingredientService;

    [HttpGet]
    public async Task<IActionResult> GetAll(){
        var ingredients = await _ingredientService.GetAllIngredientsAsync();
        return Ok(ingredients);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name){
        var ingredient = await _ingredientService.GetIngredientByNameAsync(name);
        if (User == null){
            return NotFound();
        }
        return Ok(ingredient);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Ingredient ingredient){
        var result = await _ingredientService.CreateIngredientAsync(ingredient);
        if (result > 0){
            return CreatedAtAction(nameof(Get), new { name = ingredient.Name }, ingredient);
        }
        return BadRequest();
    }
}