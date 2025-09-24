using Test.Builder;

namespace Test.BuilderTest;

public class IngredientBuilderTest
{
    [Fact]
    public void IngredientBuilder_ReturnsIngredient_ThatIsNotNull()
    {
        //Arrange
        var builder = new IngredientBuilder();

        //Act
        var ingredient = builder.Build();

        //Assert
        Assert.NotNull(ingredient);
    }

    [Theory]
    [InlineData(2, "Pasta", 45, 123)]
    [InlineData(5, "Tomato", 232, 212321)]
    public void IngredientBuilder_ReturnsIngredient_WithExpectedValues(int id, string name, int userId, int foodImageId)
    {
        //Arrange
        var builder = new IngredientBuilder()
            .WithId(id)
            .WithName(name)
            .WithUserId(userId)
            .WithImageId(foodImageId);

        //Act
        var ingredient = builder.Build();

        //Assert
        Assert.Equal(id, ingredient.Id);
        Assert.Equal(name, ingredient.Name);
        Assert.Equal(userId, ingredient.User_id);
        Assert.Equal(foodImageId, ingredient.Food_image_id);
    }
}