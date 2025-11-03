using Test.Builder;

namespace Test.BuilderTest;

public class MealBuilderTest
{
    [Fact]
    public void MealBuilder_ReturnsMeal_ThatIsNotNull()
    {
        //Arrange
        var builder = new MealBuilder();

        //Act
        var meal = builder.Build();

        //Assert
        Assert.NotNull(meal);
    }

    [Theory]
    [InlineData(3, "Pizza", 2312, 12312, "23/11/2020")]
    [InlineData(8, "Burger", 587, 8775455, "08/12/2011")]
    public void MealBuilder_ReturnsMeal_WithExpectedValues(int id, string name, int userId, int foodImageId, string date)
    {
        //Arrange
        var builder = new MealBuilder()
            .WithId(id)
            .WithName(name)
            .WithUserId(userId)
            .WithFoodImageId(foodImageId)
            .WithDate(date);

        //Act
        var meal = builder.Build();

        Assert.Equal(id, meal.Id);
        Assert.Equal(name, meal.Name);
        Assert.Equal(userId, meal.User_id);
        Assert.Equal(foodImageId, meal.Food_image_id);
        Assert.Equal(date, meal.Date);
    }
}