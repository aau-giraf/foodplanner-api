using FoodplannerApi.Helpers;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller
{
    /**
    * The controller for the PackedIngredient class.
    */
    public class PackedIngredientController (PackedIngredientService packedIngredientService, AuthService authService) : BaseController
    {
        private readonly PackedIngredientService _packedIngredientService = packedIngredientService;
        private readonly AuthService _authService = authService;

        // Get all packed ingredients
        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAll() {
            // Calls the service to get all packed ingredients
            var packedIngredients = await _packedIngredientService.GetAllPackedIngredientsAsync(); 
            return Ok(packedIngredients); // Returns the packed ingredients with a 200 OK status
        }

        // Get a specific packed ingredient by id
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Get(int id) {
            // Calls the service to get the packed ingredient by ID
            var packedIngredient = await _packedIngredientService.GetPackedIngredientByIdAsync(id); 
            if (packedIngredient == null)
            {
                return NotFound();
            }
            return Ok(packedIngredient);
        }

        // Create a new packed ingredient
        [HttpPost]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> Create([FromBody] PackedIngredientContainer packIngredient) {
            // Calls the service to create a new packed ingredient
            var mealId = packIngredient.Meal_id;
            var ingredientId = packIngredient.Ingredient_id;
            var result = await _packedIngredientService.CreatePackedIngredientAsync(mealId, ingredientId); 
            if (result > 0)
            {
                var createdPI = await _packedIngredientService.GetPackedIngredientByIdAsync(result);
                return CreatedAtAction(nameof(Get), new { id = result }, createdPI);
            }
            return BadRequest();
        }

        // Update an existing packed ingredient
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Update([FromBody] PackedIngredient packedIngredient, int id){
            var result = await _packedIngredientService.UpdatePackedIngredientAsync(packedIngredient, id);
            if (result > 0){
                var changedPackedIngredient = await _packedIngredientService.GetPackedIngredientByIdAsync(id);
                return Ok(changedPackedIngredient);
            }
            return BadRequest();
        }

        // Delete a packed ingredient by id
        [HttpDelete("{id}")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> Delete(int id) {
            // Calls the service to delete the packed ingredient by ID
            var result = await _packedIngredientService.DeletePackedIngredientAsync(id);
            if (result > 0)
            {
                return NoContent();
            }
            return NotFound();
        }
    }

    public class PackedIngredientContainer{
        public int Meal_id {get; set;}
        public int Ingredient_id { get; set; }
    }
}