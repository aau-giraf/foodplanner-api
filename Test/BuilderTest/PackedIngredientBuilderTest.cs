using Test.Builder;

namespace Test.BuilderTest;

public class PackedIngredientBuilderTest
{
    [Fact]
    public void PackedIngredientBuilder_ReturnsPackedIngredient_ThatIsNotNull()
    {
        //Arrange
        var builder = new PackedIngredientBuilder();

        //Act
        var packedIngredient = builder.Build();

        //Assert
        Assert.NotNull(packedIngredient);
    }

    [Theory]
    [InlineData(3, 343, 2312, 12312)]
    [InlineData(343, 2123, 231434412, 1231234232)]

    public void PackedIngredientBuilder_ReturnsPackedIngredient_WithExpectedValues(int id, int ingredientId, int mealId, int orderNumber)
    {
        //Arrange
        var builder = new PackedIngredientBuilder()
            .WithId(id)
            .WithIngredientId(ingredientId)
            .WithMealId(mealId)
            .WithOrderNumber(orderNumber);
        
        //Act
        var packedIngredient = builder.Build();
        
        //Assert
        
        Assert.Equal(id, packedIngredient.Id);
        Assert.Equal(ingredientId, packedIngredient.Ingredient_id);
        Assert.Equal(mealId, packedIngredient.Meal_id);
        Assert.Equal(orderNumber, packedIngredient.order_number);
    }
}