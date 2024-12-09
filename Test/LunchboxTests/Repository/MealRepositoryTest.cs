using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;

namespace Test.Repository;

[Collection("Database collection")]
public class MealRepositoryTest(DatabaseFixture fixture)
{
    private readonly DatabaseFixture _fixture = fixture;

    [Fact]
    public async void GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        //Setup
        MealRepository mealRep = new(_fixture.GetFactory());
        
        //Attempt
        IEnumerable<Meal> actual = await mealRep.GetAllAsync();

        //Cleanup
        _fixture.ResetDatabase();

        //Verify
        Assert.Empty(actual);
    }

    [Fact]
    public async void GetAllAsync_OneMealInDatabase_ReturnsOneMeal()
    {
        //Setup
        MealRepository mealRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        Meal meal = new() { Id = 0, Name = "test", User_id = userId, Food_image_id = imageId, Date = "test"};
        
        //Attempt
        await mealRep.InsertAsync(meal, userId);
        IEnumerable<Meal> actual = await mealRep.GetAllAsync();

        //Cleanup
        _fixture.ResetDatabase();

        //Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetAllByUserAsync_EmptyDatabase_ReturnsEmptyList()
    {
        //Setup
        MealRepository mealRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        
        //Attempt
        IEnumerable<Meal> actual = await mealRep.GetAllByUserAsync(userId, "test");
        
        //Verify
        Assert.Empty(actual);
    }

    [Fact]
    public async void GetByUserAllAsync_OneMealInDatabase_ReturnsOneMeal()
    {
        //Setup
        MealRepository mealRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        Meal meal = new() { Id = 0, Name = "test", User_id = userId, Food_image_id = imageId, Date = "test"};
        
        //Attempt
        await mealRep.InsertAsync(meal, userId);
        IEnumerable<Meal> actual = await mealRep.GetAllByUserAsync(userId, "test");

        //Cleanup
        _fixture.ResetDatabase();

        //Verify
        Assert.Single(actual);
    }

    [Fact]
    public async void GetByIdAsync_EmptyDatabase_ReturnsNull()
    {
        //Setup
        MealRepository mealRep = new(_fixture.GetFactory());
        
        //Attempt
        Meal actual = await mealRep.GetByIdAsync(0);
        
        //Verify
        Assert.Null(actual);
    }

    [Fact]
    public async void GetByIdAsync_OneMealInDatabase_ReturnsTheSameMeal()
    {
        //Setup
        MealRepository mealRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        Meal meal = new() { Id = 0, Name = "test", User_id = userId, Food_image_id = imageId, Date = "test"};
        string expected = "test";
        
        //Attempt
        int mealId = await mealRep.InsertAsync(meal, userId);
        Meal actual = await mealRep.GetByIdAsync(mealId);

        //Clean Up
        _fixture.ResetDatabase();

        //Verify
        Assert.Equal(expected, actual.Name);
    }

    [Fact]
    public async void UpdateAsync_OneMealInDatabase_ReturnsTheUpdatedMeal()
    {
        //Setup
        MealRepository mealRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        Meal meal = new() { Id = 0, Name = "old test", User_id = userId, Food_image_id = imageId, Date = "test"};
        Meal updatedMeal = new() { Id = 0, Name = "new test", User_id = userId, Food_image_id = imageId, Date = "test"};
        string expected = "new test";
        
        //Attempt
        int mealId = await mealRep.InsertAsync(meal, userId);
        await mealRep.UpdateAsync(updatedMeal, mealId);
        Meal actual = await mealRep.GetByIdAsync(mealId);
        await mealRep.DeleteAsync(mealId);

        //Clean Up
        _fixture.ResetDatabase();

        //Verify
        Assert.Equal(expected, actual.Name);
    }

    [Fact]
    public async void DeleteAsync_OneMealInDatabase_ReturnsNull()
    {
        //Setup
        MealRepository mealRep = new(_fixture.GetFactory());
        int userId = await _fixture.CreateTestUser();
        int imageId = await _fixture.CreateTestImage(userId);
        Meal meal = new() { Id = 0, Name = "test", User_id = userId, Food_image_id = imageId, Date = "test"};
        
        //Attempt
        int mealId = await mealRep.InsertAsync(meal, userId);
        await mealRep.DeleteAsync(mealId);
        Meal actual = await mealRep.GetByIdAsync(mealId);

        //Clean Up
        _fixture.ResetDatabase();

        //Verify
        Assert.Null(actual);
    }
}