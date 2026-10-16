using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodplannerApi.Controller;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test.LunchboxTests.Controller;

public class PackedPackedIngredientControllerTests
{
    private readonly Mock<IPackedService> mockPackedService = new();
    private readonly PackedIngredientController packedIngredientController;

    public PackedPackedIngredientControllerTests()
    {
        packedIngredientController = new PackedIngredientController(mockPackedService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var packedIngredients = new List<PackedDTO>
        {
            new() { Ingredient_id = 1, Meal_id = 1, Order_number = 1 },
            new() { Ingredient_id = 2, Meal_id = 1, Order_number = 2 }
        };

        mockPackedService.Setup(service => service.GetAllPackedAsync()).ReturnsAsync(packedIngredients);

        // Act
        var result = await packedIngredientController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPacked = Assert.IsAssignableFrom<IEnumerable<PackedDTO>>(okResult.Value).ToList();
        Assert.Equal(2, returnedPacked.Count);
        Assert.Equal(1, returnedPacked[0].Ingredient_id);
        Assert.Equal(2, returnedPacked[1].Ingredient_id);
        Assert.All(returnedPacked, packed => Assert.Equal(1, packed.Meal_id));
        mockPackedService.Verify(service => service.GetAllPackedAsync(), Times.Once());
    }

    [Fact]
    public async Task Get_ReturnsOkObjectResult_WhenPackedIngredientDoesExist()
    {
        // Arrange
        int mealId = 1;
        var packedIngredients = new List<PackedDTO>
        {
            new() { Ingredient_id = 1, Meal_id = mealId, Order_number = 1 },
            new() { Ingredient_id = 2, Meal_id = mealId, Order_number = 2 }
        };

        mockPackedService.Setup(service => service.GetAllPackedByMealIdAsync(mealId)).ReturnsAsync(packedIngredients);

        // Act
        var result = await packedIngredientController.Get(mealId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPacked = Assert.IsAssignableFrom<IEnumerable<PackedDTO>>(okResult.Value).ToList();
        Assert.Equal(2, returnedPacked.Count);
        Assert.Equal(1, returnedPacked[0].Ingredient_id);
        Assert.Equal(2, returnedPacked[1].Ingredient_id);
        Assert.All(returnedPacked, packed => Assert.Equal(mealId, packed.Meal_id));
        mockPackedService.Verify(service => service.GetAllPackedByMealIdAsync(mealId), Times.Once());
    }

    [Fact]
    public async Task Get_ReturnsNotFoundResult_WhenPackedIngredientDoesNotExist()
    {
        // Arrange
        int mealId = 999;
        mockPackedService.Setup(service => service.GetAllPackedByMealIdAsync(mealId))
            .ReturnsAsync((IEnumerable<PackedDTO>)null!);

        // Act
        var result = await packedIngredientController.Get(mealId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockPackedService.Verify(service => service.GetAllPackedByMealIdAsync(mealId), Times.Once());
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenPackedIngredientIsCreated()
    {
        // Arrange
        int createdId = 1;
        var packedIngredient = new PackedDTO { Ingredient_id = 1, Meal_id = 1, Order_number = 1 };
        mockPackedService.Setup(service => service.CreatePackedAsync(packedIngredient)).ReturnsAsync(createdId);

        // Act
        var result = await packedIngredientController.Create(packedIngredient);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(PackedIngredientController.Get), createdResult.ActionName);
        Assert.Equal(createdId, createdResult.RouteValues!["id"]);
        Assert.Same(packedIngredient, createdResult.Value);
        mockPackedService.Verify(service => service.CreatePackedAsync(packedIngredient), Times.Once());
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenPackedIngredientIsNotCreated()
    {
        // Arrange
        var packedIngredient = new PackedDTO { Ingredient_id = 1, Meal_id = 1, Order_number = 1 };
        mockPackedService.Setup(service => service.CreatePackedAsync(packedIngredient)).ReturnsAsync(0);

        // Act
        var result = await packedIngredientController.Create(packedIngredient);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        mockPackedService.Verify(service => service.CreatePackedAsync(packedIngredient), Times.Once());
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenPackedIngredientIsUpdated()
    {
        // Arrange
        var oldPacked = new PackedDTO { Ingredient_id = 1, Meal_id = 1, Order_number = 1 };
        var newPacked = new PackedDTO { Ingredient_id = 1, Meal_id = 1, Order_number = 2 };
        mockPackedService.Setup(service => service.UpdatePackedAsync(oldPacked, newPacked)).ReturnsAsync(1);

        // Act
        var result = await packedIngredientController.Update(oldPacked, newPacked);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(1, Assert.IsType<int>(okResult.Value));
        mockPackedService.Verify(service => service.UpdatePackedAsync(oldPacked, newPacked), Times.Once());
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenPackedIngredientIsNotUpdated()
    {
        // Arrange
        var oldPacked = new PackedDTO { Ingredient_id = 1, Meal_id = 1, Order_number = 1 };
        var newPacked = new PackedDTO { Ingredient_id = 1, Meal_id = 1, Order_number = 2 };
        mockPackedService.Setup(service => service.UpdatePackedAsync(oldPacked, newPacked)).ReturnsAsync(0);

        // Act
        var result = await packedIngredientController.Update(oldPacked, newPacked);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        mockPackedService.Verify(service => service.UpdatePackedAsync(oldPacked, newPacked), Times.Once());
    }

    [Fact]
    public async Task Delete_ReturnsOkObjectResult_WhenPackedIngredientDoesExist()
    {
        // Arrange
        var packedIngredient = new PackedDTO { Ingredient_id = 1, Meal_id = 1, Order_number = 1 };
        mockPackedService.Setup(service => service.DeletePackedAsync(packedIngredient)).ReturnsAsync(1);

        // Act
        var result = await packedIngredientController.Delete(packedIngredient);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockPackedService.Verify(service => service.DeletePackedAsync(packedIngredient), Times.Once());
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundResult_WhenPackedIngredientDoesNotExist()
    {
        // Arrange
        var packedIngredient = new PackedDTO { Ingredient_id = 1, Meal_id = 1, Order_number = 1 };
        mockPackedService.Setup(service => service.DeletePackedAsync(packedIngredient)).ReturnsAsync(0);

        // Act
        var result = await packedIngredientController.Delete(packedIngredient);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockPackedService.Verify(service => service.DeletePackedAsync(packedIngredient), Times.Once());
    }
}
