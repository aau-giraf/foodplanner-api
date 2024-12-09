using Moq;
using FoodplannerModels.Lunchbox;
using FoodplannerApi.Controller;
using FoodplannerApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;

namespace Test.Controller;

public class PackedPackedIngredientControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var mockPackedIngredientService = new Mock<IPackedIngredientService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        var packedIngredients = new List<PackedIngredientProperDTO>
        {
            new() { Ingredient_id = 1, Meal_id = 1 },
            new() { Ingredient_id = 2, Meal_id = 1 }
        };

        mockPackedIngredientService
            .Setup(repo => repo.GetAllPackedIngredientsAsync())
            .ReturnsAsync(packedIngredients);

        var pakcedIngredientController = new PackedIngredientController(mockPackedIngredientService.Object, authService);

        // Act
        var result = await pakcedIngredientController.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_ReturnsOkObjectResult_WhenPackedIngredientDoesExist()
    {
        // Arrange
        var mockPackedIngredientService = new Mock<IPackedIngredientService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int packedIngredientId = 1;
        PackedIngredient packedIngredient = new() { Id = packedIngredientId, Ingredient_id = 1, Meal_id = 1, order_number = 1 };

        mockPackedIngredientService
            .Setup(repo => repo.GetPackedIngredientByIdAsync(packedIngredientId))
            .ReturnsAsync(packedIngredient);

        var packedIngredientController = new PackedIngredientController(mockPackedIngredientService.Object, authService);

        // Act
        var result = await packedIngredientController.Get(packedIngredientId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundResult_WhenPackedIngredientDoesNotExist()
    {
        // Arrange
        var mockPackedIngredientService = new Mock<IPackedIngredientService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int packedIngredientId = 1;
        PackedIngredient? packedIngredient = null;

        mockPackedIngredientService
            .Setup(repo => repo.GetPackedIngredientByIdAsync(packedIngredientId))
            .ReturnsAsync(packedIngredient);

        var packedIngredientController = new PackedIngredientController(mockPackedIngredientService.Object, authService);

        // Act
        var result = await packedIngredientController.Get(packedIngredientId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenPackedIngredientIsCreated()
    {
        // Arrange
        var mockPackedIngredientService = new Mock<IPackedIngredientService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int ingredientId = 1;
        int mealId = 1;
        PackedIngredientProperDTO packedIngredientDTO = new() { Ingredient_id = ingredientId, Meal_id = mealId};
        int packedIngredientId = 1;
        PackedIngredient packedIngredient = new() { Id = packedIngredientId, Ingredient_id = 1, Meal_id = 1, order_number = 1 };

        mockPackedIngredientService
            .Setup(repo => repo.CreatePackedIngredientAsync(mealId, ingredientId))
            .ReturnsAsync(packedIngredientId);
        mockPackedIngredientService
            .Setup(repo => repo.GetPackedIngredientByIdAsync(packedIngredientId))
            .ReturnsAsync(packedIngredient);

        var packedIngredientController = new PackedIngredientController(mockPackedIngredientService.Object, authService);

        // Act
        var result = await packedIngredientController.Create(packedIngredientDTO);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenPackedIngredientIsNotCreated()
    {
        // Arrange
        var mockPackedIngredientService = new Mock<IPackedIngredientService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int ingredientId = 1;
        int mealId = 1;
        PackedIngredientProperDTO packedIngredientDTO = new() { Ingredient_id = ingredientId, Meal_id = mealId};

        mockPackedIngredientService
            .Setup(repo => repo.CreatePackedIngredientAsync(mealId, ingredientId))
            .ReturnsAsync(0);

        var packedIngredientController = new PackedIngredientController(mockPackedIngredientService.Object, authService);

        // Act
        var result = await packedIngredientController.Create(packedIngredientDTO);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenPackedIngredientIsUpdated()
    {
        // Arrange
        var mockPackedIngredientService = new Mock<IPackedIngredientService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int packedIngredientId = 1;
        PackedIngredient packedIngredient = new() { Id = packedIngredientId, Ingredient_id = 1, Meal_id = 1, order_number = 1 };

        mockPackedIngredientService
            .Setup(repo => repo.UpdatePackedIngredientAsync(packedIngredient, packedIngredientId))
            .ReturnsAsync(packedIngredientId);
        mockPackedIngredientService
            .Setup(repo => repo.GetPackedIngredientByIdAsync(packedIngredientId))
            .ReturnsAsync(packedIngredient);

        var packedIngredientController = new PackedIngredientController(mockPackedIngredientService.Object, authService);

        // Act
        var result = await packedIngredientController.Update(packedIngredient, packedIngredientId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenPackedIngredientIsNotUpdated()
    {
        // Arrange
        var mockPackedIngredientService = new Mock<IPackedIngredientService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int packedIngredientId = 1;
        PackedIngredient packedIngredient = new() { Id = packedIngredientId, Ingredient_id = 1, Meal_id = 1, order_number = 1 };

        mockPackedIngredientService
            .Setup(repo => repo.UpdatePackedIngredientAsync(packedIngredient, packedIngredientId))
            .ReturnsAsync(0);

        var packedIngredientController = new PackedIngredientController(mockPackedIngredientService.Object, authService);

        // Act
        var result = await packedIngredientController.Update(packedIngredient, packedIngredientId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOkObjectResult_WhenPackedIngredientDoesExist()
    {
        // Arrange
        var mockPackedIngredientService = new Mock<IPackedIngredientService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int packedIngredientId = 1;

        mockPackedIngredientService
            .Setup(repo => repo.DeletePackedIngredientAsync(packedIngredientId))
            .ReturnsAsync(1);

        var packedIngredientController = new PackedIngredientController(mockPackedIngredientService.Object, authService);

        // Act
        var result = await packedIngredientController.Delete(packedIngredientId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundResult_WhenPackedIngredientDoesNotExist()
    {
        // Arrange
        var mockPackedIngredientService = new Mock<IPackedIngredientService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int packedIngredientId = 1;

        mockPackedIngredientService
            .Setup(repo => repo.DeletePackedIngredientAsync(packedIngredientId))
            .ReturnsAsync(0);

        var packedIngredientController = new PackedIngredientController(mockPackedIngredientService.Object, authService);

        // Act
        var result = await packedIngredientController.Delete(packedIngredientId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}