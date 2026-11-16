
using FoodplannerApi.Controller;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Integration.Controllers;

public class PackedTests
{
    private readonly Mock<IPackedRepository> mockRepository = new();
    private readonly PackedIngredientController controller;

    public PackedTests()
    {
        var service = new PackedService(mockRepository.Object);
        controller = new PackedIngredientController(service);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAll_ShouldReturnCorrectPackedIngredients()
    {
        // Arrange
        int mealId = 42;
        var expected1 = CreatePacked(mealId, 1, 1);
        var expected2 = CreatePacked(mealId, 2, 2);

        var expectedPacked = new[] { expected1, expected2 };

        mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedPacked);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var packed = Assert.IsAssignableFrom<IEnumerable<PackedDTO>>(okResult.Value).ToList();

        Assert.Equal(2, packed.Count);
        Assert.Equal(expected1.Meal_id, packed[0].Meal_id);
        Assert.Equal(expected1.Ingredient_id, packed[0].Ingredient_id);
        Assert.Equal(expected1.order_no, packed[0].order_no);

        Assert.Equal(expected2.Meal_id, packed[1].Meal_id);
        Assert.Equal(expected2.Ingredient_id, packed[1].Ingredient_id);
        Assert.Equal(expected2.order_no, packed[1].order_no);

        mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    private static PackedDTO CreatePacked(int mealId, int ingredientId, int orderNo)
    {
        return new PackedDTO
        {
            Meal_id = mealId,
            Ingredient_id = ingredientId,
            order_no = orderNo
        };
    }
}
