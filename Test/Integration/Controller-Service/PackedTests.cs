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
    public async Task Create_ShouldCreatePackedIngredient()
    {
        // Arrange
        int generatedId = 42;
        var packed = CreatePacked(10, 2, 1);

        mockRepository.Setup(r => r.InsertAsync(packed))
            .ReturnsAsync(generatedId);

        // Act
        var result = await controller.Create(packed);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var createdPacked = Assert.IsType<PackedDTO>(createdResult.Value);

        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(generatedId, createdResult.RouteValues!["id"]);
        Assert.Equal(packed.Meal_id, createdPacked.Meal_id);
        Assert.Equal(packed.Ingredient_id, createdPacked.Ingredient_id);
        Assert.Equal(packed.order_no, createdPacked.order_no);

        mockRepository.Verify(r => r.InsertAsync(packed), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Create_ShouldReturnBadRequestWhenInsertFails()
    {
        // Arrange
        var packed = CreatePacked(10, 2, 1);

        mockRepository.Setup(r => r.InsertAsync(packed))
            .ReturnsAsync(0);

        // Act
        var result = await controller.Create(packed);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        mockRepository.Verify(r => r.InsertAsync(packed), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Update_ShouldUpdatePackedIngredient()
    {
        // Arrange
        int mealId = 42;
        int ingredientId = 2;
        var updated = CreatePacked(mealId, ingredientId, 3);

        mockRepository.Setup(r => r.UpdateAsync(
                It.Is<PackedDTO>(p => p.Meal_id == mealId &&
                                      p.Ingredient_id == ingredientId &&
                                      p.order_no == 0),
                updated))
            .ReturnsAsync(1);

        // Act
        var result = await controller.Update(updated, ingredientId, mealId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, Assert.IsType<int>(okResult.Value));

        mockRepository.Verify(r => r.UpdateAsync(
            It.Is<PackedDTO>(p => p.Meal_id == mealId &&
                                  p.Ingredient_id == ingredientId &&
                                  p.order_no == 0),
            updated), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Update_ShouldReturnBadRequestWhenUpdateFails()
    {
        // Arrange
        int mealId = 42;
        int ingredientId = 2;
        var updated = CreatePacked(mealId, ingredientId, 3);

        mockRepository.Setup(r => r.UpdateAsync(
                It.Is<PackedDTO>(p => p.Meal_id == mealId &&
                                      p.Ingredient_id == ingredientId &&
                                      p.order_no == 0),
                updated))
            .ReturnsAsync(0);

        // Act
        var result = await controller.Update(updated, ingredientId, mealId);

        // Assert
        Assert.IsType<BadRequestResult>(result);

        mockRepository.Verify(r => r.UpdateAsync(
            It.Is<PackedDTO>(p => p.Meal_id == mealId &&
                                  p.Ingredient_id == ingredientId &&
                                  p.order_no == 0),
            updated), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Delete_ShouldDeletePackedIngredient()
    {
        // Arrange
        var packed = CreatePacked(42, 2, 1);

        mockRepository.Setup(r => r.DeleteAsync(packed))
            .ReturnsAsync(1);

        // Act
        var result = await controller.Delete(packed);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockRepository.Verify(r => r.DeleteAsync(packed), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Delete_ShouldReturnNotFoundWhenDeleteFails()
    {
        // Arrange
        var packed = CreatePacked(42, 2, 1);

        mockRepository.Setup(r => r.DeleteAsync(packed))
            .ReturnsAsync(0);

        // Act
        var result = await controller.Delete(packed);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockRepository.Verify(r => r.DeleteAsync(packed), Times.Once);
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
