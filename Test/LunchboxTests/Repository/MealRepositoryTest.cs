using FoodplannerModels.Lunchbox;
using FoodplannerDataAccessSql.Lunchbox;

namespace testing;

[Collection("Database collection")]
public class MealRepositoryTest(DatabaseFixture fixture)
{
    private readonly DatabaseFixture _fixture = fixture;

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
}