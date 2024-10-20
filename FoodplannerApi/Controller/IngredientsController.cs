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

    [HttpGet("{name}/{user}")]
    public async Task<IActionResult> Get(string name, string user){
        var ingredient = await _ingredientService.GetIngredientByNameAsync(name, user);
        if (User == null){
            return NotFound();
        }
        return Ok(ingredient);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Ingredient ingredient){
        var result = await _ingredientService.CreateIngredientAsync(ingredient);
        if (result > 0){
            return CreatedAtAction(nameof(Get), new { name = ingredient.Name, user = ingredient.User_ref}, ingredient);
        }
        return BadRequest();
    }

    [HttpPut("{name}/{user}")]
    public async Task<IActionResult> Update([FromBody] Ingredient ingredient, string name, string user){
        var result = await _ingredientService.UpdateIngredientAsync(ingredient, name, user);
        if (result > 0){
            var changedIngredient = await _ingredientService.GetIngredientByNameAsync(ingredient.Name, ingredient.User_ref);
            return Ok(changedIngredient);
        }
        return BadRequest();
    }

    [HttpDelete("{name}/{user}")]
    public async Task<IActionResult> Delete(string name, string user){
        var ingredient = await _ingredientService.GetIngredientByNameAsync(name, user);
        var result = await _ingredientService.DeleteIngredientAsync(name, user);
        if (result > 0){
            return Ok(ingredient);
        }
        return NotFound();
    }
}