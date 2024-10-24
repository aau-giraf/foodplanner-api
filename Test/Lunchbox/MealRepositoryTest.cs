using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;

namespace testing;

[Collection("Sequential")]
public class MealRepositoryTest
{
    [Fact]
    public async void GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
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
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        Meal meal = new Meal{Id = 0, Title = "test", User_ref = "test", Image_ref = "test", Date = "test"};
        
        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> actual = await mealRep.GetAllAsync();
        
        //Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetByIdAsync_EmptyDatabase_ReturnsNull()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        
        //Attempt
        Meal actual = await mealRep.GetByIdAsync(0);
        
        //Verify
        Assert.Null(actual);
    }

    [Fact]
    public async void GetByIdAsync_OneMealInDatabase_ReturnsTheSameMeal()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        Meal meal = new Meal{Id = 0, Title = "test", User_ref = "test", Image_ref = "test", Date = "test"};
        string expected = "test";
        
        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        Meal actual = await mealRep.GetByIdAsync(mealId);
        
        //Verify
        Assert.Equal(expected, actual.Title);
    }

    [Fact]
    public async void UpdateAsync_OneMealInDatabase_ReturnsTheUpdatedMeal()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        Meal meal = new Meal{Id = 0, Title = "old", User_ref = "test", Image_ref = "test", Date = "test"};
        Meal updatedMeal = new Meal{Id = 0, Title = "new", User_ref = "test", Image_ref = "test", Date = "test"};
        string expected = "new";
        
        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        await mealRep.UpdateAsync(updatedMeal, mealId);
        Meal actual = await mealRep.GetByIdAsync(mealId);
        await mealRep.DeleteAsync(mealId);
        
        //Verify
        Assert.Equal(expected, actual.Title);
    }

    [Fact]
    public async void DeleteAsync_OneMealInDatabase_ReturnsNull()
    {
        //Setup
        DatabaseConnection.EmptyDatabase("packed_ingredients");
        DatabaseConnection.EmptyDatabase("meals");
        MealRepository mealRep = new MealRepository(DatabaseConnection.GetConnection());
        Meal meal = new Meal{Id = 0, Title = "test", User_ref = "test", Image_ref = "test", Date = "test"};
        
        //Attempt
        await mealRep.InsertAsync(meal);
        IEnumerable<Meal> allMeals = await mealRep.GetAllAsync();
        int mealId = allMeals.FirstOrDefault().Id;
        await mealRep.DeleteAsync(mealId);
        Meal actual = await mealRep.GetByIdAsync(mealId);
        
        //Verify
        Assert.Null(actual);
    }
}