using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerApi.Controller;
using Microsoft.AspNetCore.Mvc;
using Dapper;

namespace testing;

/**
* These are technicly integration tests, since the database has not been mocked using Xunits Fixture.
* It is therefor important to check that the database SW5-10 is empty before running the tests.
*/
[Collection("Sequential")]
public class MealControllerTest
{

    [Fact]
    public async void GetAll_NoInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);

        //Attempt
        IActionResult actual = await mealCon.GetAll();

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void GetAllByUser_NoInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);

        //Attempt
        IActionResult actual = await mealCon.GetAllByUser(1, "test");

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void GetAllByUser_TwoMeals_ReturnsOkObjectResultOfOneMeal()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);
        //--New User--
        using var connection = DatabaseConnection.GetConnection().Create();
        await connection.OpenAsync();
        var sql = "INSERT INTO users (first_name, last_name, email, password, role, role_approved)\n";
        sql += $"VALUES ('Temp2', 'Temp2', 'empty', '1234', 'Test', true)";
        await connection.ExecuteAsync(sql);
        //--New User--
        Meal meal1 = new() { Id = 0, Title = "test1", User_ref = 1, Image_ref = 1, Date = "test"};
        Meal meal2 = new() { Id = 0, Title = "test2", User_ref = 2, Image_ref = 1, Date = "test"};

        //Attempt
        await mealRep.InsertAsync(meal1);
        await mealRep.InsertAsync(meal2);
        IActionResult action = await mealCon.GetAllByUser(2, "test");
        var objectResult = action as OkObjectResult;
        var actual = objectResult.Value as List<MealDTO>;

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<OkObjectResult>(action);
        Assert.Equal(meal2.Title, actual[0].Title);
    }

    [Fact]
    public async void Get_WrongInput_ReturnsNotFoundResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);

        //Attempt
        IActionResult actual = await mealCon.Get(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async void Get_CorrectInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        IActionResult actual = await mealCon.Get(mealId);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Create_CorrectInput_ReturnsCreatedAtActionResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        IActionResult actual = await mealCon.Create(meal);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<CreatedAtActionResult>(actual);
    }

    [Fact]
    public async void Update_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "old test", User_ref = 1, Image_ref = 1, Date = "test"};
        Meal updatedMeal = new() { Id = 0, Title = "new test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        IActionResult actual = await mealCon.Update(updatedMeal, mealId);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Update_WrongId_ReturnsBadRequestResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        IActionResult actual = await mealCon.Update(meal, 0);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.IsType<BadRequestResult>(actual);
    }

    [Fact]
    public async void Delete_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        IActionResult actual = await mealCon.Delete(mealId);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");
        
        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Delete_WrongId_ReturnsNotFounfResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IngredientRepository ingredientRep = new(DatabaseConnection.GetConnection());
        PackedIngredientRepository packedRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep, packedRep, ingredientRep);
        MealsController mealCon = new(mealServ);

        //Attempt
        IActionResult actual = await mealCon.Delete(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }
}