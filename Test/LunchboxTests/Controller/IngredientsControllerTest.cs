using FoodplannerApi.Controller;
using FoodplannerModels.Auth;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test.LunchboxTests.Controller;

public class IngredientsControllerTests
{
    private readonly Mock<IIngredientsService> mockIngredientsService = new();
    private readonly Mock<IAuthService> mockAuthService = new();
    private readonly IngredientsController ingredientsController;

    public IngredientsControllerTests()
    {
        ingredientsController = new IngredientsController(mockIngredientsService.Object, mockAuthService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var ingredients = new List<IngredientDTO>
        {
            new() { Id = 1, Name = "Cheese", User_id = 1, Image_id = null },
            new() { Id = 2, Name = "Bread", User_id = 1, Image_id = null }
        };
        mockIngredientsService.Setup(service => service.GetAllIngredientsAsync()).ReturnsAsync(ingredients);

        // Act
        var result = await ingredientsController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedIngredients = Assert.IsAssignableFrom<IEnumerable<IngredientDTO>>(okResult.Value).ToList();
        Assert.Equal(2, returnedIngredients.Count);
        Assert.Equal(1, returnedIngredients[0].Id);
        Assert.Equal("Cheese", returnedIngredients[0].Name);
        Assert.Equal(2, returnedIngredients[1].Id);
        Assert.Equal("Bread", returnedIngredients[1].Name);
        mockIngredientsService.Verify(service => service.GetAllIngredientsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllByUser_ReturnsOkObjectResult()
    {
        // Arrange
        const string token = "Bearer TestToken";
        int userId = 1;
        var ingredients = new List<IngredientDTO>
        {
            new() { Id = 1, Name = "Cheese", User_id = userId, Image_id = null },
            new() { Id = 2, Name = "Bread", User_id = userId, Image_id = null }
        };
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());
        mockIngredientsService.Setup(service => service.GetAllIngredientsByUserAsync(userId)).ReturnsAsync(ingredients);

        // Act
        var result = await ingredientsController.GetAllByUser(token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedIngredients = Assert.IsAssignableFrom<IEnumerable<IngredientDTO>>(okResult.Value).ToList();
        Assert.Equal(2, returnedIngredients.Count);
        Assert.All(returnedIngredients, ingredient => Assert.Equal(userId, ingredient.User_id));
        Assert.Equal("Cheese", returnedIngredients[0].Name);
        Assert.Equal("Bread", returnedIngredients[1].Name);
        mockIngredientsService.Verify(service => service.GetAllIngredientsByUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsOkObjectResult_WhenIngredientExists()
    {
        // Arrange
        int ingredientId = 1;
        var ingredient = new IngredientDTO { Id = ingredientId, Name = "Cheese", User_id = 1, Image_id = null };
        mockIngredientsService.Setup(service => service.GetIngredientByIdAsync(ingredientId)).ReturnsAsync(ingredient);

        // Act
        var result = await ingredientsController.Get(ingredientId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedIngredient = Assert.IsType<IngredientDTO>(okResult.Value);
        Assert.Equal(ingredientId, returnedIngredient.Id);
        Assert.Equal("Cheese", returnedIngredient.Name);
        Assert.Equal(1, returnedIngredient.User_id);
        mockIngredientsService.Verify(service => service.GetIngredientByIdAsync(ingredientId), Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundResult_WhenIngredientDoesNotExist()
    {
        // Arrange
        int ingredientId = 999;
        mockIngredientsService.Setup(service => service.GetIngredientByIdAsync(ingredientId)).ReturnsAsync((IngredientDTO?)null);

        // Act
        var result = await ingredientsController.Get(ingredientId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockIngredientsService.Verify(service => service.GetIngredientByIdAsync(ingredientId), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenIngredientIsCreated()
    {
        // Arrange
        const string token = "Bearer TestToken";
        int userId = 1;
        int ingredientId = 42;
        var ingredientDto = new IngredientDTO { Id = 0, Name = "Cheese", User_id = userId, Image_id = null };
        var createdIngredient = new IngredientDTO { Id = ingredientId, Name = "Cheese", User_id = userId, Image_id = null };
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());
        mockIngredientsService.Setup(service => service.CreateIngredientAsync(ingredientDto)).ReturnsAsync(ingredientId);
        mockIngredientsService.Setup(service => service.GetIngredientByIdAsync(ingredientId)).ReturnsAsync(createdIngredient);

        // Act
        var result = await ingredientsController.Create(token, ingredientDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(IngredientsController.Get), createdResult.ActionName);
        Assert.Equal(ingredientId, createdResult.RouteValues!["id"]);
        var returnedIngredient = Assert.IsType<IngredientDTO>(createdResult.Value);
        Assert.Equal(ingredientId, returnedIngredient.Id);
        Assert.Equal("Cheese", returnedIngredient.Name);
        Assert.Equal(userId, returnedIngredient.User_id);
        mockIngredientsService.Verify(service => service.CreateIngredientAsync(ingredientDto), Times.Once);
        mockIngredientsService.Verify(service => service.GetIngredientByIdAsync(ingredientId), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenIngredientIsNotCreated()
    {
        // Arrange
        const string token = "Bearer TestToken";
        int userId = 1;
        var ingredientDto = new IngredientDTO { Id = 0, Name = "Cheese", User_id = userId, Image_id = null };
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());
        mockIngredientsService.Setup(service => service.CreateIngredientAsync(ingredientDto)).ReturnsAsync(0);

        // Act
        var result = await ingredientsController.Create(token, ingredientDto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        mockIngredientsService.Verify(service => service.CreateIngredientAsync(ingredientDto), Times.Once);
        mockIngredientsService.Verify(service => service.GetIngredientByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenIngredientIsUpdated()
    {
        // Arrange
        int ingredientId = 1;
        var ingredientDto = new IngredientDTO { Id = ingredientId, Name = "Updated Cheese", User_id = 1, Image_id = null };
        mockIngredientsService.Setup(service => service.UpdateIngredientAsync(ingredientDto)).ReturnsAsync(1);
        mockIngredientsService.Setup(service => service.GetIngredientByIdAsync(ingredientId)).ReturnsAsync(ingredientDto);

        // Act
        var result = await ingredientsController.Update(ingredientDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedIngredient = Assert.IsType<IngredientDTO>(okResult.Value);
        Assert.Equal(ingredientId, returnedIngredient.Id);
        Assert.Equal("Updated Cheese", returnedIngredient.Name);
        Assert.Equal(1, returnedIngredient.User_id);
        mockIngredientsService.Verify(service => service.UpdateIngredientAsync(ingredientDto), Times.Once);
        mockIngredientsService.Verify(service => service.GetIngredientByIdAsync(ingredientId), Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIngredientIsNotUpdated()
    {
        // Arrange
        int ingredientId = 1;
        var ingredientDto = new IngredientDTO { Id = ingredientId, Name = "Cheese", User_id = 1, Image_id = null };
        mockIngredientsService.Setup(service => service.UpdateIngredientAsync(ingredientDto)).ReturnsAsync(0);

        // Act
        var result = await ingredientsController.Update(ingredientDto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        mockIngredientsService.Verify(service => service.UpdateIngredientAsync(ingredientDto), Times.Once);
        mockIngredientsService.Verify(service => service.GetIngredientByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ReturnsOkObjectResult_WhenIngredientDoesExist()
    {
        // Arrange
        int ingredientId = 1;
        var ingredient = new IngredientDTO { Id = ingredientId, Name = "Cheese", User_id = 1, Image_id = null };
        mockIngredientsService.Setup(service => service.GetIngredientByIdAsync(ingredientId)).ReturnsAsync(ingredient);
        mockIngredientsService.Setup(service => service.DeleteIngredientAsync(ingredientId)).ReturnsAsync(1);

        // Act
        var result = await ingredientsController.Delete(ingredientId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedIngredient = Assert.IsType<IngredientDTO>(okResult.Value);
        Assert.Equal(ingredientId, returnedIngredient.Id);
        Assert.Equal("Cheese", returnedIngredient.Name);
        mockIngredientsService.Verify(service => service.GetIngredientByIdAsync(ingredientId), Times.Once);
        mockIngredientsService.Verify(service => service.DeleteIngredientAsync(ingredientId), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundResult_WhenIngredientDoesNotExist()
    {
        // Arrange
        int ingredientId = 999;
        mockIngredientsService.Setup(service => service.GetIngredientByIdAsync(ingredientId)).ReturnsAsync((IngredientDTO?)null);
        mockIngredientsService.Setup(service => service.DeleteIngredientAsync(ingredientId)).ReturnsAsync(0);

        // Act
        var result = await ingredientsController.Delete(ingredientId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockIngredientsService.Verify(service => service.GetIngredientByIdAsync(ingredientId), Times.Once);
        mockIngredientsService.Verify(service => service.DeleteIngredientAsync(ingredientId), Times.Once);
    }
}
