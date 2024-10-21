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

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id){
        var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
        if (User == null){
            return NotFound();
        }
        return Ok(ingredient);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Ingredient ingredient){
        var result = await _ingredientService.CreateIngredientAsync(ingredient);
        if (result > 0){
            return CreatedAtAction(nameof(Get), new { id = ingredient.Id}, ingredient);
        }
        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Ingredient ingredient, int id){
        var result = await _ingredientService.UpdateIngredientAsync(ingredient, id);
        if (result > 0){
            var changedIngredient = await _ingredientService.GetIngredientByIdAsync(id);
            return Ok(changedIngredient);
        }
        return BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
        var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
        var result = await _ingredientService.DeleteIngredientAsync(id);
        if (result > 0){
            return Ok(ingredient);
        }
        return NotFound();
    }
}