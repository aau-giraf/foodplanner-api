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

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id){
        var meal = await _mealService.GetMealByIdAsync(id);
        if (User == null){
            return NotFound();
        }
        return Ok(meal);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Meal meal){
        var result = await _mealService.CreateMealAsync(meal);
        if (result > 0){
            return CreatedAtAction(nameof(Get), new { id = meal.Id }, meal);
        }
        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Meal meal, int id){
        var result = await _mealService.UpdateMealAsync(meal, id);
        if (result > 0){
            var changedMeal = await _mealService.GetMealByIdAsync(id);
            return Ok(changedMeal);
        }
        return BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
        var meal = await _mealService.GetMealByIdAsync(id);
        var result = await _mealService.DeleteMealAsync(id);
        if (result > 0){
            return Ok(meal);
        }
        return NotFound();
    }

}