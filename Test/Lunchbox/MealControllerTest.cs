using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerApi.Controller;
using Microsoft.AspNetCore.Mvc;

namespace testing;

public class MealControllerTest
{
    [Fact]
    public async void GetAll_EmptyDatabase_ReturnsEmptyList()
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
        IActionResult actual = await mealCon.GetAll();

        //Verify
        Assert.IsType<OkObjectResult>(actual);
    }
}