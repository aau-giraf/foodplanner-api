using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Dapper;
using FoodplannerDataAccessSql.Account;
using System.Diagnostics;

namespace testing;

[Collection("Sequential")]
public class MealRepositoryTest
{   
    [Fact]
    public async void GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IEnumerable<Meal> expected = [];
        
        //Attempt
        IEnumerable<Meal> actual = await mealRep.GetAllAsync();
        
        //Verify
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void GetAllAsync_OneMealInDatabase_ReturnsOneMeal()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};
        
        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> actual = await mealRep.GetAllAsync();

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetAllByUserAsync_EmptyDatabase_ReturnsEmptyList()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        IEnumerable<Meal> expected = [];
        
        //Attempt
        IEnumerable<Meal> actual = await mealRep.GetAllByUserAsync(1, "test");
        
        //Verify
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void GetByUserAllAsync_OneMealInDatabase_ReturnsOneMeal()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};
        
        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> actual = await mealRep.GetAllByUserAsync(1, "test");

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetByIdAsync_EmptyDatabase_ReturnsNull()
    {
        //Setup
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        
        //Attempt
        Meal actual = await mealRep.GetByIdAsync(0);
        
        //Verify
        Assert.Null(actual);
    }

    [Fact]
    public async void GetByIdAsync_OneMealInDatabase_ReturnsTheSameMeal()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};
        string expected = "test";
        
        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        Meal actual = await mealRep.GetByIdAsync(mealId);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.Equal(expected, actual.Title);
    }

    [Fact]
    public async void UpdateAsync_OneMealInDatabase_ReturnsTheUpdatedMeal()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        Meal meal = new() { Id = 0, Title = "old test", User_ref = 1, Image_ref = 1, Date = "test"};
        Meal updatedMeal = new() { Id = 0, Title = "new test", User_ref = 1, Image_ref = 1, Date = "test"};
        string expected = "new test";
        
        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        await mealRep.UpdateAsync(updatedMeal, mealId);
        Meal actual = await mealRep.GetByIdAsync(mealId);
        await mealRep.DeleteAsync(mealId);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.Equal(expected, actual.Title);
    }

    [Fact]
    public async void DeleteAsync_OneMealInDatabase_ReturnsNull()
    {
        //Setup
        await DatabaseConnection.SetupTempUserAndImage();
        await DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new(DatabaseConnection.GetConnection());
        Meal meal = new() { Id = 0, Title = "test", User_ref = 1, Image_ref = 1, Date = "test"};
        
        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        await mealRep.DeleteAsync(mealId);
        Meal actual = await mealRep.GetByIdAsync(mealId);

        //Clean Up
        await DatabaseConnection.EmptyDatabase("meals");
        await DatabaseConnection.EmptyDatabase("food_image");
        await DatabaseConnection.EmptyDatabase("users");

        //Verify
        Assert.Null(actual);
    }
}