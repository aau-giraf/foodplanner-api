
using FoodplannerApi.Controller;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Integration.Controllers;

public class IngredientsTests
{
    private readonly Mock<IIngredientsRepository> mockRepository = new();
    private readonly Mock<IAuthService> mockAuthService = new();
    private readonly IngredientsController controller;

    public IngredientsTests()
    {
        var service = new IngredientsService(mockRepository.Object);
        controller = new IngredientsController(service, mockAuthService.Object);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Get_ShouldReturnCorrectIngredient()
    {
        // Arrange
        int ingredientId = 42;
        var expected = CreateIngredient(ingredientId, "Test Ingredient", 12345);

        mockRepository.Setup(r => r.GetByIdAsync(ingredientId))
            .ReturnsAsync(expected);

        // Act
        var result = await controller.Get(ingredientId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ingredient = Assert.IsType<IngredientDTO>(okResult.Value);

        Assert.Equal(expected.Id, ingredient.Id);
        Assert.Equal(expected.Name, ingredient.Name);
        Assert.Equal(expected.User_id, ingredient.User_id);

        mockRepository.Verify(r => r.GetByIdAsync(ingredientId), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Get_ShouldReturnNotFoundIngredient()
    {
        // Arrange
        int ingredientId = 42;
        mockRepository.Setup(r => r.GetByIdAsync(ingredientId))
        .ReturnsAsync((IngredientDTO?)null);

        // Act
        var result = await controller.Get(ingredientId);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        mockRepository.Verify(r => r.GetByIdAsync(ingredientId), Times.Once);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAll_ShouldReturnCorrectIngredients()
    {
        // Arrange
        int ingredientId1 = 42;
        int ingredientId2 = 32;
        var expected1 = CreateIngredient(ingredientId1, "Test Ingredient1", 12345);
        var expected2 = CreateIngredient(ingredientId2, "Test Ingredient2", 12345);

        var expectedIngredients = new[] { expected1, expected2 };

        mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedIngredients);

        // Act
        var result = await controller.GetAllIngredients();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ingredients = Assert.IsAssignableFrom<IEnumerable<IngredientDTO>>(okResult.Value).ToList();

        Assert.Equal(2, ingredients.Count);
        Assert.Equal(expected1.Id, ingredients[0].Id);
        Assert.Equal(expected1.Name, ingredients[0].Name);
        Assert.Equal(expected2.Id, ingredients[1].Id);
        Assert.Equal(expected2.Name, ingredients[1].Name);

        mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAll_ShouldReturnCorrectUserIngredients()
    {
        // Arrange
        int userId = 12345;
        string token = "Bearer test-token";

        int ingredientId1 = 42;
        int ingredientId2 = 32;
        var expected1 = CreateIngredient(ingredientId1, "Test Ingredient1", userId);
        var expected2 = CreateIngredient(ingredientId2, "Test Ingredient2", userId);

        var expectedIngredients = new[] { expected1, expected2 };

        mockAuthService.Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());

        mockRepository.Setup(r => r.GetAllByUserAsync(userId))
            .ReturnsAsync(expectedIngredients);

        // Act
        var result = await controller.GetAllByUser(token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ingredients = Assert.IsAssignableFrom<IEnumerable<IngredientDTO>>(okResult.Value).ToList();

        Assert.Equal(2, ingredients.Count);
        Assert.Equal(expected1.Id, ingredients[0].Id);
        Assert.Equal(expected1.Name, ingredients[0].Name);
        Assert.Equal(expected2.Id, ingredients[1].Id);
        Assert.Equal(expected2.Name, ingredients[1].Name);

        mockAuthService.Verify(a => a.RetrieveIdFromJwtToken(token), Times.Once);
        mockRepository.Verify(r => r.GetAllByUserAsync(userId), Times.Once);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task Create_ShouldCreateIngredient()
    {
        // Arrange
        int userId = 12345;
        int generatedId = 42;
        string token = "Bearer test-token";

        var ingredient = CreateIngredient(0, "Test Ingredient", userId);
        var expected = CreateIngredient(generatedId, "Test Ingredient", userId);

        mockAuthService.Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());

        mockRepository.Setup(r => r.InsertAsync(ingredient))
            .ReturnsAsync(generatedId);

        mockRepository.Setup(r => r.GetByIdAsync(generatedId))
            .ReturnsAsync(expected);

        // Act
        var result = await controller.Create(token, ingredient);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var createdIngredient = Assert.IsType<IngredientDTO>(createdResult.Value);

        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(generatedId, createdIngredient.Id);
        Assert.Equal(ingredient.Name, createdIngredient.Name);
        Assert.Equal(userId, createdIngredient.User_id);

        mockAuthService.Verify(a => a.RetrieveIdFromJwtToken(token), Times.Once);
        mockRepository.Verify(r => r.InsertAsync(ingredient), Times.Once);
        mockRepository.Verify(r => r.GetByIdAsync(generatedId), Times.Once);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsync_ShouldUpdateIngredient()
    {
        // Arrange
        int ingredientId = 42;
        var updated = CreateIngredient(ingredientId, "Updated Ingredient", 12345);

        mockRepository.Setup(r => r.UpdateAsync(updated))
            .ReturnsAsync(1);

        mockRepository.Setup(r => r.GetByIdAsync(ingredientId))
            .ReturnsAsync(updated);

        // Act
        var result = await controller.Update(updated);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ingredient = Assert.IsType<IngredientDTO>(okResult.Value);

        Assert.Equal(updated.Id, ingredient.Id);
        Assert.Equal(updated.Name, ingredient.Name);
        Assert.Equal(updated.User_id, ingredient.User_id);

        mockRepository.Verify(r => r.UpdateAsync(updated), Times.Once);
        mockRepository.Verify(r => r.GetByIdAsync(ingredientId), Times.Once);
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteAsync_ShouldDeleteIngredient()
    {
        // Arrange
        int ingredientId = 42;
        var deleted = CreateIngredient(ingredientId, "Deleted Ingredient", 12345);

        mockRepository.Setup(r => r.DeleteAsync(ingredientId))
            .ReturnsAsync(1);

        mockRepository.Setup(r => r.GetByIdAsync(ingredientId))
            .ReturnsAsync(deleted);

        // Act
        var result = await controller.Delete(ingredientId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var ingredient = Assert.IsType<IngredientDTO>(okResult.Value);

        Assert.Equal(deleted.Id, ingredient.Id);
        Assert.Equal(deleted.Name, ingredient.Name);
        Assert.Equal(deleted.User_id, ingredient.User_id);

        mockRepository.Verify(r => r.DeleteAsync(ingredientId), Times.Once);
        mockRepository.Verify(r => r.GetByIdAsync(ingredientId), Times.Once);
    }

    private static IngredientDTO CreateIngredient(int id, string name, int userId)
    {
        return new IngredientDTO
        {
            Id = id,
            Name = name,
            User_id = userId,
            Image_id = null
        };
    }
}
