using Moq;
using FoodplannerModels.Lunchbox;
using FoodplannerApi.Controller;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;

namespace Test.LunchboxTests.Controller;

public class IngredientsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = Mock.Of<IAuthService>();

        var ingredients = new List<Ingredient>
        {
            new() { Id = 1, Name = "Cheese", User_id = 1 },
            new() { Id = 2, Name = "Bread", User_id = 1 }
        };

        mockIngredientService
            .Setup(repo => repo.GetAllIngredientsAsync())
            .ReturnsAsync(ingredients);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService);

        // Act
        var result = await ingredientsController.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAllByUser_ReturnsOkObjectResult()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = new Mock<IAuthService>();

        int userId = 1;
        var ingredients = new List<Ingredient>
        {
            new() { Id = 1, Name = "Cheese", User_id = userId },
            new() { Id = 2, Name = "Bread", User_id = userId }
        };
        var user = new User() { Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = UserRole.Parent, RoleApproved = true };

        var JWTToken = "Bearer TestToken";
        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(JWTToken))
            .Returns(userId.ToString());

        mockIngredientService
            .Setup(repo => repo.GetAllIngredientsByUserAsync(userId))
            .ReturnsAsync(ingredients);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService.Object);

        // Act
        var result = await ingredientsController.GetAllByUser(JWTToken);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_ReturnsOkObjectResult_WhenIngredientIsInserted()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = Mock.Of<IAuthService>();

        int ingredientId = 1;
        Ingredient ingredient = new() { Id = ingredientId, Name = "Cheese", User_id = 1 };

        mockIngredientService
            .Setup(repo => repo.GetIngredientByIdAsync(ingredientId))
            .ReturnsAsync(ingredient);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService);

        // Act
        var result = await ingredientsController.Get(ingredientId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundResult_WhenIngredientIsNotInserted()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = Mock.Of<IAuthService>();

        int ingredientId = 1;
        Ingredient ingredient = null!;

        mockIngredientService
            .Setup(repo => repo.GetIngredientByIdAsync(ingredientId))
            .ReturnsAsync(ingredient);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService);

        // Act
        var result = await ingredientsController.Get(ingredientId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenIngredientIsCreated()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = new Mock<IAuthService>();

        int ingredientId = 1;
        int userId = 1;
        IngredientDTO ingredientDTO = new() { Name = "test"};
        Ingredient ingredient = new() { Id = ingredientId, Name = "Cheese", User_id = userId };
        var user = new User() { Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = UserRole.Parent, RoleApproved = true };
        
        var JWTToken = "Bearer TestToken";
        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(JWTToken))
            .Returns(userId.ToString());
        
        mockIngredientService
            .Setup(repo => repo.CreateIngredientAsync(ingredientDTO, userId))
            .ReturnsAsync(ingredientId);
        mockIngredientService
            .Setup(repo => repo.GetIngredientByIdAsync(ingredientId))
            .ReturnsAsync(ingredient);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService.Object);

        // Act
        var result = await ingredientsController.Create(JWTToken, ingredientDTO);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenIngredientIsNotCreated()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = new Mock<IAuthService>();

        int userId = 1;
        IngredientDTO ingredientDTO = new() { Name = "test"};
        var user = new User() { Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = UserRole.Parent, RoleApproved = true };

        var JWTToken = "Bearer TestToken";
        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(JWTToken))
            .Returns(userId.ToString());

        mockIngredientService
            .Setup(repo => repo.CreateIngredientAsync(ingredientDTO, userId))
            .ReturnsAsync(0);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService.Object);

        // Act
        var result = await ingredientsController.Create(JWTToken, ingredientDTO);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenIngredientIsUpdated()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = Mock.Of<IAuthService>();

        int ingredientId = 1;
        Ingredient ingredient = new() { Id = ingredientId, Name = "Cheese", User_id = 1 };

        mockIngredientService
            .Setup(repo => repo.UpdateIngredientAsync(ingredient, ingredientId))
            .ReturnsAsync(ingredientId);
        mockIngredientService
            .Setup(repo => repo.GetIngredientByIdAsync(ingredientId))
            .ReturnsAsync(ingredient);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService);

        // Act
        var result = await ingredientsController.Update(ingredient, ingredientId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenIngredientIsNotUpdated()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = Mock.Of<IAuthService>();

        int ingredientId = 1;
        Ingredient ingredient = new() { Id = ingredientId, Name = "Cheese", User_id = 1 };

        mockIngredientService
            .Setup(repo => repo.UpdateIngredientAsync(ingredient, ingredientId))
            .ReturnsAsync(0);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService);

        // Act
        var result = await ingredientsController.Update(ingredient, ingredientId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOkObjectResult_WhenIngredientDoesExist()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = Mock.Of<IAuthService>();

        int ingredientId = 1;
        Ingredient ingredient = new() { Id = ingredientId, Name = "Cheese", User_id = 1 };

        mockIngredientService
            .Setup(repo => repo.DeleteIngredientAsync(ingredientId))
            .ReturnsAsync(1);
        mockIngredientService
            .Setup(repo => repo.GetIngredientByIdAsync(ingredientId))
            .ReturnsAsync(ingredient);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService);

        // Act
        var result = await ingredientsController.Delete(ingredientId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundResult_WhenIngredientDoesNotExist()
    {
        // Arrange
        var mockIngredientService = new Mock<IIngredientService>();
        var mockAuthService = Mock.Of<IAuthService>();

        int ingredientId = 1;
        Ingredient ingredient = null!;

        mockIngredientService
            .Setup(repo => repo.DeleteIngredientAsync(ingredientId))
            .ReturnsAsync(0);
        mockIngredientService
            .Setup(repo => repo.GetIngredientByIdAsync(ingredientId))
            .ReturnsAsync(ingredient);

        var ingredientsController = new IngredientsController(mockIngredientService.Object, mockAuthService);

        // Act
        var result = await ingredientsController.Delete(ingredientId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}