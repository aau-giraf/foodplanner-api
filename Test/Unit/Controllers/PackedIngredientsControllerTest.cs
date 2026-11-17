using FoodplannerApi.Controller;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Test.Unit.Controller;

public class PackedPackedIngredientControllerTests
{
    private readonly Mock<IPackedService> mockPackedService = new();
    private readonly PackedController packedIngredientController;

    public PackedPackedIngredientControllerTests()
    {
        packedIngredientController = new PackedController(mockPackedService.Object);
    }

[Fact]
[Trait("Category", "Unit")]
public async Task Create_ReturnsCreatedResult_WhenPackedIngredientIsCreated()
{
    // Arrange
    int createdId = 1;
    var packedIngredient = new Packed
    {
        Ingredient_id = 1,
        Meal_id = 1,
        order_no = 1
    };

    mockPackedService
        .Setup(service => service.CreatePackedAsync(packedIngredient))
        .ReturnsAsync(createdId);

    // Act
    var result = await packedIngredientController.Create(packedIngredient);

    // Assert
    var createdResult = Assert.IsType<ObjectResult>(result);
    Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    Assert.Same(packedIngredient, createdResult.Value);

    mockPackedService.Verify(
        service => service.CreatePackedAsync(packedIngredient),
        Times.Once());
}

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Create_ReturnsCreatedAtActionResult_WhenPackedIngredientIsNotCreated()
    {
        // Arrange
        var packedIngredient = new Packed { Ingredient_id = 1, Meal_id = 1, order_no = 1 };
        mockPackedService.Setup(service => service.CreatePackedAsync(packedIngredient)).ReturnsAsync(0);

        // Act
        var result = await packedIngredientController.Create(packedIngredient);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        mockPackedService.Verify(service => service.CreatePackedAsync(packedIngredient), Times.Once());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Update_ReturnsOkObjectResult_WhenPackedIngredientIsUpdated()
    {
        // Arrange
        var oldPacked = new Packed { Ingredient_id = 1, Meal_id = 1, order_no = 0 };
        var newPacked = new Packed { Ingredient_id = 1, Meal_id = 1, order_no = 2 };
        mockPackedService.Setup(service => service.UpdatePackedAsync(
            It.Is<Packed>(p =>
                p.Ingredient_id == oldPacked.Ingredient_id &&
                p.Meal_id == oldPacked.Meal_id &&
                p.order_no == oldPacked.order_no),
            It.Is<Packed>(p =>
                p.Ingredient_id == newPacked.Ingredient_id &&
                p.Meal_id == newPacked.Meal_id &&
                p.order_no == newPacked.order_no)))
            .ReturnsAsync(1);

        // Act
        var result = await packedIngredientController.Update(newPacked, 1, 1);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        mockPackedService.Verify(service => service.UpdatePackedAsync(
            It.Is<Packed>(p =>
                p.Ingredient_id == 1 &&
                p.Meal_id == 1 &&
                p.order_no == 0),
            It.Is<Packed>(p =>
                p.Ingredient_id == 1 &&
                p.Meal_id == 1 &&
                p.order_no == 2)),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Update_ReturnsBadRequest_WhenPackedIngredientIsNotUpdated()
    {
        // Arrange
        var oldPacked = new Packed { Ingredient_id = 1, Meal_id = 1, order_no = 0 };
        var newPacked = new Packed { Ingredient_id = 1, Meal_id = 1, order_no = 2 };
        mockPackedService.Setup(service => service.UpdatePackedAsync(
            It.Is<Packed>(p =>
                p.Ingredient_id == oldPacked.Ingredient_id &&
                p.Meal_id == oldPacked.Meal_id &&
                p.order_no == oldPacked.order_no),
            It.Is<Packed>(p =>
                p.Ingredient_id == newPacked.Ingredient_id &&
                p.Meal_id == newPacked.Meal_id &&
                p.order_no == newPacked.order_no)))
            .ReturnsAsync(0);

        // Act
        var result = await packedIngredientController.Update(newPacked, 1, 1);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        mockPackedService.Verify(service => service.UpdatePackedAsync(
            It.Is<Packed>(p =>
                p.Ingredient_id == 1 &&
                p.Meal_id == 1 &&
                p.order_no == 0),
            It.Is<Packed>(p =>
                p.Ingredient_id == 1 &&
                p.Meal_id == 1 &&
                p.order_no == 2)),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Delete_ReturnsOkObjectResult_WhenPackedIngredientDoesExist()
    {
        // Arrange
        var packedIngredient = new Packed { Ingredient_id = 1, Meal_id = 1, order_no = 1 };
        mockPackedService.Setup(service => service.DeletePackedAsync(packedIngredient)).ReturnsAsync(1);

        // Act
        var result = await packedIngredientController.Delete(packedIngredient);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockPackedService.Verify(service => service.DeletePackedAsync(packedIngredient), Times.Once());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Delete_ReturnsNotFoundResult_WhenPackedIngredientDoesNotExist()
    {
        // Arrange
        var packedIngredient = new Packed { Ingredient_id = 1, Meal_id = 1, order_no = 1 };
        mockPackedService.Setup(service => service.DeletePackedAsync(packedIngredient)).ReturnsAsync(0);

        // Act
        var result = await packedIngredientController.Delete(packedIngredient);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockPackedService.Verify(service => service.DeletePackedAsync(packedIngredient), Times.Once());
    }
}
