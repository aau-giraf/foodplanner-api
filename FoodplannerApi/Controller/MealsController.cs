using FoodplannerApi.Helpers;
using FoodplannerModels.Account;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

/**
* The MealsController class handles CRUD (Create, Read, Update, Delete) operations for the Meal entity.
* It uses the MealService to interact with the database and process meal-related requests.
*/
public class MealsController (MealService mealService, AuthService authService) : BaseController
{
    // Private field to hold the injected MealService.
    private readonly MealService _mealService = mealService;
    private readonly AuthService _authService = authService;

    // Get all meals
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(){
        var meals = await _mealService.GetAllMealsAsync();
        return Ok(meals);
    }

    // Get all meals by user id
    [HttpGet("{date}")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetAllByUser([FromHeader(Name = "Authorization")] string token, string date){
        try {    
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id)) {
                return BadRequest(new ErrorResponse {Message = ["Id er ikke et tal"]});
            }
            var meals = await _mealService.GetAllMealsByUserAsync(id, date);
            return Ok(meals);
        }
        catch (InvalidOperationException e){
            return BadRequest(new ErrorResponse {Message = [e.Message]});
        }
    }

    // Get a specific meal by ID
    [HttpGet("{id}")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> Get(int id){
        var meal = await _mealService.GetMealByIdAsync(id);
        if (meal == null){ // Returns 404 if the meal is not found
            return NotFound();
        }
        return Ok(meal);
    }

    // Create a new meal
    [HttpPost]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] MealContainer mealContainer){
        try {    
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id)) {
                return BadRequest(new ErrorResponse {Message = ["Id er ikke et tal"]});
            }
            Meal meal = new() {Id = mealContainer.Id, Title = mealContainer.Title, User_ref = id, Image_ref = mealContainer.Image_ref, Date = mealContainer.Date};
            var result = await _mealService.CreateMealAsync(meal);
            if (result > 0){ // Returns 201 with an object of the new meal
                var createdMeal = await _mealService.GetMealByIdAsync(result);
                return CreatedAtAction(nameof(Get), new { id = result }, createdMeal);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e){
            return BadRequest(new ErrorResponse {Message = [e.Message]});
        }
    }

    // Update an existing meal
    [HttpPut("{id}")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> Update([FromHeader(Name = "Authorization")] string token, [FromBody] Meal meal, int id){
        try {    
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int user_id)) {
                return BadRequest(new ErrorResponse {Message = ["Id er ikke et tal"]});
            }
            meal.User_ref = user_id;
            var result = await _mealService.UpdateMealAsync(meal, id);
            if (result > 0){ // Returns the updated meal with a 200 OK status
                var changedMeal = await _mealService.GetMealByIdAsync(id);
                return Ok(changedMeal);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e){
            return BadRequest(new ErrorResponse {Message = [e.Message]});
        }
    }

    // Delete a meal by ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> Delete(int id){
        var meal = await _mealService.GetMealByIdAsync(id);
        var result = await _mealService.DeleteMealAsync(id);
        if (result > 0){ // Returns the deleted meal with a 200 OK status
            return Ok(meal);
        }
        return NotFound();
    }

    public class MealContainer{
        public int Id {get; set;}
        public string Title {get; set;}
        public int? Image_ref {get; set;}
        public string Date {get; set;}
    }
}