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
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        MealService mealServ = new MealService(mealRep);
        MealsController mealCon = new MealsController(mealServ);

        //Attempt
        IActionResult actual = await mealCon.GetAll();

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }

    [Fact]
    public async void Get_WrongInput_ReturnsNotFoundResult()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        MealService mealServ = new MealService(mealRep);
        MealsController mealCon = new MealsController(mealServ);

        //Attempt
        IActionResult actual = await mealCon.Get(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async void Get_CorrectInput_ReturnsOkObjectResult()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        MealService mealServ = new MealService(mealRep);
        MealsController mealCon = new MealsController(mealServ);
        Meal meal = new Meal{Id = 0, Title = "test", User_ref = "test", Image_ref = "test", Date = "test"};

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
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        MealService mealServ = new MealService(mealRep);
        MealsController mealCon = new MealsController(mealServ);
        Meal meal = new Meal{Id = 0, Title = "test", User_ref = "test", Image_ref = "test", Date = "test"};

        //Attempt
        IActionResult actual = await mealCon.Create(meal);

        //Verify
        Assert.IsType<CreatedAtActionResult>(actual);
    }

    [Fact]
    public async void Update_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        MealService mealServ = new MealService(mealRep);
        MealsController mealCon = new MealsController(mealServ);
        Meal meal = new Meal{Id = 0, Title = "test", User_ref = "old", Image_ref = "test", Date = "test"};
        Meal updatedMeal = new Meal{Id = 0, Title = "test", User_ref = "new", Image_ref = "test", Date = "test"};

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
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        MealService mealServ = new MealService(mealRep);
        MealsController mealCon = new MealsController(mealServ);
        Meal meal = new Meal{Id = 0, Title = "test", User_ref = "test", Image_ref = "test", Date = "test"};

        //Attempt
        IActionResult actual = await mealCon.Update(meal, 0);

        //Verify
        Assert.IsType<BadRequestResult>(actual);
    }

    [Fact]
    public async void Delete_CorrectId_ReturnsOkObjectResult()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        MealService mealServ = new MealService(mealRep);
        MealsController mealCon = new MealsController(mealServ);
        Meal meal = new Meal{Id = 0, Title = "test", User_ref = "test", Image_ref = "test", Date = "test"};

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
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        MealService mealServ = new MealService(mealRep);
        MealsController mealCon = new MealsController(mealServ);

        //Attempt
        IActionResult actual = await mealCon.Delete(0);

        //Verify
        Assert.IsType<NotFoundResult>(actual);
    }
}