using FoodplannerApi.Controller;
using FoodplannerApi.Helpers;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Dapper;

namespace testing;

/**
* These are technicly integration tests, since the database has not been mocked using Xunits Fixture.
* It is therefor important to check that the database SW5-10 is empty before running the tests.
*/
[Collection("Sequential")]  // Indicates that the tests in this collection should run sequentially to avoid issues with shared state.
public class PackedIngredientControllerIntegrationTests
{
    private static readonly MealRepository mealRep = new(DatabaseConnection.GetConnection());
    private static readonly IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
    private static readonly PackedIngredientRepository packedIngredientRep = new(DatabaseConnection.GetConnection());
    private static readonly PackedIngredientService packedIngredientServ = new(packedIngredientRep);
    private static readonly AuthService authServ = new(WebApplication.CreateBuilder().Configuration);
    private static readonly PackedIngredientController packedIngredientCon = new(packedIngredientServ, authServ);

    // Test case to verify that the GetAll method returns a successful response with an empty database.
    [Fact]
    public async void GetAll_NoInput_ReturnsOkObjectResult()
    {
        // Setup: Empty the database for a new test environment.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");

        // Attempt: Call the GetAll method.
        IActionResult actual = await packedIngredientCon.GetAll();

        // Verify: Assert that the result is of type OkObjectResult, indicating a successful operation.
        Assert.IsType<OkObjectResult>(actual);
    }

    // Test case to verify that the Get method returns a NotFound result for an invalid ID.
    [Fact]
    public async void Get_WrongInput_ReturnsNotFoundResult()
    {
        // Setup: Empty the database for a new test environment.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");

        // Attempt: Call the Get method with an invalid ID (0).
        IActionResult actual = await packedIngredientCon.Get(0);

        // Verify: Assert that the result is of type NotFoundResult, indicating the item was not found.
        Assert.IsType<NotFoundResult>(actual);
    }

    // Test case to verify that the Get method returns a successful response for a valid ID.
    [Fact]
    public async void Get_CorrectInput_ReturnsOkObjectResult()
    {
        // Setup: Empty the database for a new test environment.
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        Meal meal = new() { Id = 1, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};       
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1};
        
        await mealRep.InsertAsync(meal);
        await ingredientRep.InsertAsync(ingredient);

        // Retrieve IDs for meal and ingredient for creating a PackedIngredient.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        int ingredientId = allIngredients.FirstOrDefault().Id;
        
        PackedIngredient packedIngredient = new() { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId};

        // Attempt: Insert the PackedIngredient and retrieve it by ID.
        await packedIngredientRep.InsertAsync(packedIngredient);
        IEnumerable<PackedIngredient> allPackedIngredients = await packedIngredientRep.GetAllAsync();
        int packedIngredientId = allPackedIngredients.FirstOrDefault().Id;
        IActionResult actual = await packedIngredientCon.Get(packedIngredientId);

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify: Assert that the result is of type OkObjectResult, confirming the item was found.
        Assert.IsType<OkObjectResult>(actual);
    }

    // Test case to verify that the Create method returns a CreatedAtActionResult when a new PackedIngredient is created successfully.
    [Fact]
    public async void Create_CorrectInput_ReturnsCreatedAtActionResult()
    {
        // Setup: Empty the database for a new test environment.
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        Meal meal = new() { Id = 1, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};        
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1};
        
        await mealRep.InsertAsync(meal);
        await ingredientRep.InsertAsync(ingredient);

        // Returns IDs for meal and ingredient.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        int ingredientId = allIngredients.FirstOrDefault().Id;
        
        PackedIngredient packedIngredient = new() { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId};

        // Attempt: Call the Create method with the PackedIngredient.
        IActionResult actual = await packedIngredientCon.Create(packedIngredient);

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify: Assert that the result is of type CreatedAtActionResult, confirming the item was created.
        Assert.IsType<CreatedAtActionResult>(actual);
    }

    // Test case to verify that the Update method returns OkObjectResult when an existing PackedIngredient is updated successfully.
    [Fact]
    public async void Update_CorrectId_ReturnsOkObjectResult()
    {
        // Setup: Empty the database for a new test environment.
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        Meal meal = new() { Id = 1, Title = "old test", User_ref = 1, Image_ref = 1, Date = "test"};
        Meal mealnew = new() { Id = 2, Title = "new test", User_ref = 1, Image_ref = 1, Date = "test"};               
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1};
        
        await mealRep.InsertAsync(meal);
        await mealRep.InsertAsync(mealnew);
        await ingredientRep.InsertAsync(ingredient);

        // Return IDs for meals and ingredients.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        int mealNewID = allMeals.Skip(1).FirstOrDefault().Id;
        int ingredientId = allIngredients.FirstOrDefault().Id;
        
        PackedIngredient packedIngredient = new() { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId};
        PackedIngredient updatedPackedIngredient = new() { Id = 3, Meal_ref= mealNewID, Ingredient_ref= ingredientId};

        // Attempt: Insert the original PackedIngredient and then update it.
        await packedIngredientRep.InsertAsync(packedIngredient);
        IEnumerable<PackedIngredient> allPackedIngredients = await packedIngredientRep.GetAllAsync();
        int packedIngredientId = allPackedIngredients.FirstOrDefault().Id;
        IActionResult actual = await packedIngredientCon.Update(updatedPackedIngredient, packedIngredientId);

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify: Assert that the result is of type OkObjectResult, confirming the item was updated.
        Assert.IsType<OkObjectResult>(actual);
    }

    // Test case to verify that the Update method returns BadRequestResult when an incorrect ID is provided.
    [Fact]
    public async void Update_WrongId_ReturnsBadRequestResult()
    {
        // Setup: Empty the database for a new test environment.
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        Meal meal = new() { Id = 1, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};        
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1};
        
        await mealRep.InsertAsync(meal);
        await ingredientRep.InsertAsync(ingredient);

        // Return IDs for meal and ingredient.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        int ingredientId = allIngredients.FirstOrDefault().Id;
        
        PackedIngredient packedIngredient = new() { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId};

        // Attempt: Call the Update method with an invalid ID (0).
        IActionResult actual = await packedIngredientCon.Update(packedIngredient, 0);

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify: Assert that the result is of type BadRequestResult, indicating the request was not valid.
        Assert.IsType<BadRequestResult>(actual);
    }

    // Test case to verify that the Delete method returns OkObjectResult when an existing PackedIngredient is deleted successfully.
    [Fact]
    public async void Delete_CorrectId_ReturnsOkObjectResult()
    {
        // Setup: Empty the database for a new test environment.
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        Meal meal = new() { Id = 1, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};        
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1};
        
        await mealRep.InsertAsync(meal);
        await ingredientRep.InsertAsync(ingredient);

        // Return IDs for meal and ingredient.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        int ingredientId = allIngredients.FirstOrDefault().Id;
        
        PackedIngredient packedIngredient = new() { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId};

        // Attempt: Insert the PackedIngredient and then delete it.
        await packedIngredientRep.InsertAsync(packedIngredient);
        IEnumerable<PackedIngredient> allPackedIngredients = await packedIngredientRep.GetAllAsync();
        int packedIngredientId = allPackedIngredients.FirstOrDefault().Id;
        IActionResult actual = await packedIngredientCon.Delete(packedIngredientId);

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify: Assert that the result is of type NoContentResult, indicating successful deletion.
        Assert.IsType<NoContentResult>(actual);
    }

    // Test case to verify that the Delete method returns NotFoundResult when an invalid ID is provided.
    [Fact]
    public async void Delete_WrongId_ReturnsNotFounfResult()
    {
        // Setup: Empty the database for a new test environment.
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        Meal meal = new() { Id = 1, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};        
        Ingredient ingredient = new() { Id = 1, Name = "test", User_ref = 1, Image_ref = 1};
        
        await mealRep.InsertAsync(meal);
        await ingredientRep.InsertAsync(ingredient);

        // Return IDs for meal and ingredient.
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        IEnumerable<Ingredient> allIngredients = await ingredientRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        int ingredientId = allIngredients.FirstOrDefault().Id;
        
        PackedIngredient packedIngredient = new() { Id = 2, Meal_ref = mealId, Ingredient_ref = ingredientId};

        // Attempt: Call the Delete method with an invalid ID (0).
        IActionResult actual = await packedIngredientCon.Delete(0);

        //Clean Up: Empty the database so the test does not affect other tests.
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("ingredients");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        // Verify: Assert that the result is of type NotFoundResult, indicating the item was not found.
        Assert.IsType<NotFoundResult>(actual);
    }
}
