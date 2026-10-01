using FoodplannerModels.Account;
using FoodplannerModels.Lunchbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Auth;

// Adds the MealsController to the FoodPlannerApi.controller namespace
namespace FoodplannerApi.Controller;

public class MealsController(IMealService mealService, IAuthService authService) : BaseController
{

    // URL: api/Meals/GetAll
    // Retrieves all meals.
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<MealDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var meals = await mealService.GetAllMealsAsync();
        return Ok(meals);
    }

    // URL: api/Meals/GetAllByUser/{date}
    [HttpGet("{date}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IEnumerable<MealDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllByUser([FromHeader(Name = "Authorization")] string token, string date)
    {
        try
        {
            // Retrieve the user ID from the JWT token.
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Retrieve all meals for the user on the specified date.
            var meals = await mealService.GetAllMealsByUserAsync(id, date);
            return Ok(meals);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Meals/TeacherGetUserMeals
    // (change this in the future so route is /api/Meals/TeacherGetUserMeals/{date}/{id})
    [HttpGet]
    [Authorize(Roles = "Teacher, Admin")]
    [ProducesResponseType(typeof(IEnumerable<MealDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TeacherGetUserMeals(string date, int id)
    {
        var meals = await mealService.GetAllMealsByUserAsync(id, date);
        return Ok(meals);
    }

    // URL: api/Meals/Get/getmeal/{id}
    // Get a specific meal by ID
    [HttpGet("getmeal/{id}")]
    [Authorize(Roles = "Child, Parent, Teacher")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var meal = await mealService.GetMealByIdAsync(id);
        if (meal == null)
        {
            return NotFound();
        }
        return Ok(meal);
    }

    // URL: api/Meals/Create
    // Create a new meal from MealCreateDTO and return the created meal with a 201 Created status.
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] MealCreateDTO meal)
    {
        try
        {
            // Retrieve the user ID from the JWT token.
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Create the meal and return the created meal with a 201 Created status.
            var result = await mealService.CreateMealAsync(meal, id);
            if (result > 0)
            {
                var createdMeal = await mealService.GetMealByIdAsync(result);
                return CreatedAtAction(nameof(Get), new { id = result }, createdMeal);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Meals/Update/{id}
    // Update an existing meal by ID with MealDTO and return the updated meal with a 200 OK status.
    [HttpPut("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromHeader(Name = "Authorization")] string token, [FromBody] MealDTO mealDto, int id)
    {
        try
        {
            // Retrieve the user ID from the JWT token.
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int user_id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Update the meal and return the updated meal with a 200 OK status.
            mealDto.UserId = user_id;
            var result = await mealService.UpdateMealAsync(mealDto, id);
            if (result > 0)
            {
                var changedMeal = await mealService.GetMealByIdAsync(id);
                return Ok(changedMeal);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Meals/Delete/{id}
    // Delete a meal by ID and return the deleted meal with a 200 OK status.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(MealDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var meal = await mealService.GetMealByIdAsync(id);
        var result = await mealService.DeleteMealAsync(id);
        if (result > 0)
        {
            return Ok(meal);
        }
        return NotFound();
    }

    // URL: api/Meals/GetAllTemplates
    // Get all meal templates for the authenticated user.
    [HttpGet]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IEnumerable<MealDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllTemplates([FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            // Retrieve the user ID from the JWT token.
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Retrieve all meal templates for the user and return them with a 200 OK status.
            var templates = await mealService.GetAllTemplatesByUserAsync(id);
            return Ok(templates);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Meals/UpdateTemplateStatus/{id}/template
    // Update the template status of a meal by ID and return a success message with a 200 OK status.
    [HttpPut("{id}/template")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTemplateStatus([FromHeader(Name = "Authorization")] string token, int id, [FromBody] bool template)
    {
        try
        {
            // Retrieve the user ID from the JWT token.
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Update the template status of the meal and return a success message with a 200 OK status.
            var result = await mealService.UpdateTemplateStatusAsync(id, template, userId);
            if (result > 0)
            {
                return Ok(new { Message = "Template status updated successfully" });
            }
            return BadRequest(new ErrorResponse { Message = ["Could not update template status"] });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Meals/GetUniqueIngredientsFromMeals
    // Get unique ingredients from a list of meal IDs for the authenticated user.
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(IEnumerable<IngredientDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUniqueIngredientsFromMeals([FromHeader(Name = "Authorization")] string token, [FromBody] List<int> mealIds)
    {
        try
        {
            // Retrieve the user ID from the JWT token.
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Validate that meal IDs are provided.
            if (mealIds == null || !mealIds.Any())
            {
                return BadRequest(new ErrorResponse { Message = ["Meal IDs are required"] });
            }

            // Retrieve unique ingredients from the specified meals for the user and return them with a 200 OK status.
            var ingredients = await mealService.GetUniqueIngredientsFromMealsAsync(mealIds, userId);
            return Ok(ingredients);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }
}