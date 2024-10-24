using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

/**
* The MealsController class handles CRUD (Create, Read, Update, Delete) operations for the Meal entity.
* It uses the MealService to interact with the database and process meal-related requests.
*/
public class MealsController (MealService mealService) : BaseController
{
    // Private field to hold the injected MealService.
    private readonly MealService _mealService = mealService;

    // Get all meals
    [HttpGet]
    public async Task<IActionResult> GetAll(){
        var meals = await _mealService.GetAllMealsAsync();
        return Ok(meals);
    }

    // Get a specific meal by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id){
        var meal = await _mealService.GetMealByIdAsync(id);
        if (meal == null){ // Returns 404 if the meal is not found
            return NotFound();
        }
        return Ok(meal);
    }

    // Create a new meal
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Meal meal){
        var result = await _mealService.CreateMealAsync(meal);
        if (result > 0){ // Returns 201 with an object of the new meal
            return CreatedAtAction(nameof(Get), new { id = meal.Id }, meal);
        }
        return BadRequest();
    }

    // Update an existing meal
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Meal meal, int id){
        var result = await _mealService.UpdateMealAsync(meal, id);
        if (result > 0){ // Returns the updated meal with a 200 OK status
            var changedMeal = await _mealService.GetMealByIdAsync(id);
            return Ok(changedMeal);
        }
        return BadRequest();
    }

    // Delete a meal by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
        var meal = await _mealService.GetMealByIdAsync(id);
        var result = await _mealService.DeleteMealAsync(id);
        if (result > 0){ // Returns the deleted meal with a 200 OK status
            return Ok(meal);
        }
        return NotFound();
    }

}