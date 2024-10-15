using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

/**
* The controller for the Meal class.
*/
public class MealsController (MealService mealService) : BaseController
{
    private readonly MealService _mealService = mealService;

    [HttpGet]
    public async Task<IActionResult> GetAll(){
        var meals = await _mealService.GetAllMealsAsync();
        return Ok(meals);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name){
        var meal = await _mealService.GetMealByNameAsync(name);
        if (User == null){
            return NotFound();
        }
        return Ok(meal);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Meal meal){
        var result = await _mealService.CreateMealAsync(meal);
        if (result > 0){
            return CreatedAtAction(nameof(Get), new { meal_name = meal.Meal_name }, meal);
        }
        return BadRequest();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name){
        var result = await _mealService.DeleteMealAsync(name);
        if (result > 0){
            return NoContent();
        }
        return NotFound();
    }

}