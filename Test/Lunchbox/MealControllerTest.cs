using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerApi.Controller;
using Microsoft.AspNetCore.Mvc;

namespace testing;

[Collection("Sequential")]
public class MealControllerTest
{

    [Fact]
    public async void GetAll_NoInput_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep);
        MealsController mealCon = new(mealServ);

        //Attempt
        IActionResult actual = await mealCon.GetAll();

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Get_WrongInput_ReturnsNotFoundResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep);
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
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        IActionResult actual = await mealCon.Get(mealId);

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Create_CorrectInput_ReturnsCreatedAtActionResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        IActionResult actual = await mealCon.Create(meal);

        //Verify
        Assert.IsType<CreatedAtActionResult>(actual);
    }

    [Fact]
    public async void Update_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "old test", User_ref = 1, Image_ref = 1, Date = "test"};
        Meal updatedMeal = new() { Id = 0, Title = "new test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        IActionResult actual = await mealCon.Update(updatedMeal, mealId);

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Update_WrongId_ReturnsBadRequestResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        IActionResult actual = await mealCon.Update(meal, 0);

        //Verify
        Assert.IsType<BadRequestResult>(actual);
    }

    [Fact]
    public async void Delete_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep);
        MealsController mealCon = new(mealServ);
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};

        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        IActionResult actual = await mealCon.Delete(mealId);

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Delete_WrongId_ReturnsNotFounfResult()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        MealService mealServ = new(mealRep);
        MealsController mealCon = new(mealServ);

        //Attempt
        IActionResult actual = await mealCon.Delete(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async void Z() //The tests are called alphabeticly, so this is called Z to force it to be last
    {
        await DatabaseConnection.EmptyDatabase("packed_ingredients");
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        Assert.True(true);
    }
}