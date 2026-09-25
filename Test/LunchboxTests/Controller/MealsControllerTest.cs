using Moq;
using FoodplannerModels.Lunchbox;
using FoodplannerApi.Controller;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;

namespace Test.LunchboxTests.Controller;

public class MealsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = Mock.Of<IAuthService>();

        var meals = new List<MealResponseDTO>
        {
            new() { Id = 1, Name = "Pizza", Date = "test", UserId = 1, Ingredients = [] },
            new() { Id = 2, Name = "Burger", Date = "test", UserId = 1, Ingredients = [] }
        };

        mockMealService
            .Setup(repo => repo.GetAllMealsAsync())
            .ReturnsAsync(meals);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService);

        // Act
        var result = await mealsController.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAllByUser_ReturnsOkObjectResult()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = new Mock<IAuthService>();
        int userId = 1;
        string date = "test";
        var meals = new List<MealResponseDTO>
        {
            new() { Id = 1, Name = "Pizza", Date = date, UserId = userId,Ingredients = [] },
            new() { Id = 2, Name = "Burger", Date = date, UserId = userId, Ingredients = [] }
        };
        var user = new User() { Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = UserRole.Parent, RoleApproved = true };


        var JWTToken = "Bearer TestToken";
        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(JWTToken))
            .Returns(userId.ToString());

        mockMealService
            .Setup(repo => repo.GetAllMealsByUserAsync(userId, date))
            .ReturnsAsync(meals);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService.Object);

        // Act
        var result = await mealsController.GetAllByUser(JWTToken, date);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task TeacherGetUserMeals_ReturnsOkObjectResult()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = Mock.Of<IAuthService>();
        int userId = 1;

        string date = "test";
        var meals = new List<MealResponseDTO>
        {
            new() { Id = 1, Name = "Pizza", Date = date, UserId = userId, Ingredients = [] },
            new() { Id = 2, Name = "Burger", Date = date, UserId = userId, Ingredients = [] }
        };
        var user = new User() { Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = UserRole.Parent, RoleApproved = true };

        mockMealService
            .Setup(repo => repo.GetAllMealsByUserAsync(userId, date))
            .ReturnsAsync(meals);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService);

        // Act
        var result = await mealsController.TeacherGetUserMeals(date, userId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_ReturnsOkObjectResult_WhenMealDoesExist()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = Mock.Of<IAuthService>();

        int mealId = 1;
        var meal = new MealResponseDTO() { Id = mealId, Name = "Pizza", Date = "test", UserId = 1, Ingredients = [] };

        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService);

        // Act
        var result = await mealsController.Get(mealId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundResult_WhenMealDoesNotExist()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = Mock.Of<IAuthService>();

        int mealId = 1;
        MealResponseDTO meal = null!;

        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService);

        // Act
        var result = await mealsController.Get(mealId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenMealIsCreated()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = new Mock<IAuthService>();

        int mealId = 1;
        int userId = 1;

        MealCreateDTO mealDTO = new() { Name = "Pizza", Date = "test" };
        MealResponseDTO meal = new() { Id = mealId, Name = "Pizza", Date = "test", UserId = userId, Ingredients = [] };
        var user = new User() { Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = UserRole.Parent, RoleApproved = true };

        var JWTToken = "Bearer TestToken";
        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(JWTToken))
            .Returns(userId.ToString());

        mockMealService
            .Setup(repo => repo.CreateMealAsync(mealDTO, userId))
            .ReturnsAsync(mealId);
        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService.Object);

        // Act
        var result = await mealsController.Create(JWTToken, mealDTO);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenMealIsNotCreated()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = new Mock<IAuthService>();

        MealCreateDTO mealDTO = new() { Name = "Pizza", Date = "test" };
        int userId = 1;
        var user = new User() { Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = UserRole.Parent, RoleApproved = true };

        var JWTToken = "Bearer TestToken";
        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(JWTToken))
            .Returns(userId.ToString());

        mockMealService
            .Setup(repo => repo.CreateMealAsync(mealDTO, userId))
            .ReturnsAsync(0);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService.Object);

        // Act
        var result = await mealsController.Create(JWTToken, mealDTO);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenMealIsUpdated()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = new Mock<IAuthService>();
        var userId = 1;

        int mealId = 1;
        MealDTO meal = new()
        {
            Id = mealId,
            Name = "Pizza",
            Date = "test",
            UserId = userId,
            Ingredients = Enumerable.Empty<PackedIngredientDTO>()
        };
        MealResponseDTO mealDto = new() { Id = mealId, Name = "Pizza", Date = "test", UserId = userId, Ingredients = [] };
        var user = new User() { Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = UserRole.Parent, RoleApproved = true };

        var JWTToken = "Bearer TestToken";
        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(JWTToken))
            .Returns(user.Id.ToString());

        mockMealService
            .Setup(repo => repo.UpdateMealAsync(meal, mealId))
            .ReturnsAsync(mealId);
        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(mealDto);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService.Object);

        // Act
        var result = await mealsController.Update(JWTToken, meal, mealId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenMealIsNotUpdated()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = new Mock<IAuthService>();

        int mealId = 1;
        int userId = 1;
        MealDTO mealDto = new()
        {
            Id = mealId,
            Name = "Pizza",
            Date = "test",
            UserId = userId,
            Ingredients = Enumerable.Empty<PackedIngredientDTO>()
        };
        var user = new User() { Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = UserRole.Parent, RoleApproved = true };
        var JWTToken = "Bearer TestToken";
        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(JWTToken))
            .Returns(user.Id.ToString());

        mockMealService
            .Setup(repo => repo.UpdateMealAsync(mealDto, mealId))
            .ReturnsAsync(0);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService.Object);

        // Act
        var result = await mealsController.Update(JWTToken, mealDto, mealId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOkObjectResult_WhenMealExist()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = Mock.Of<IAuthService>();

        int mealId = 1;
        MealResponseDTO meal = new() { Id = mealId, Name = "Pizza", Date = "test", UserId = 1, Ingredients = [] };

        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);
        mockMealService
            .Setup(repo => repo.DeleteMealAsync(mealId))
            .ReturnsAsync(1);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService);

        // Act
        var result = await mealsController.Delete(mealId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundResult_WhenMealDoesNotExist()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var mockAuthService = new Mock<IAuthService>();

        int mealId = 1;
        MealResponseDTO meal = null!;

        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);
        mockMealService
            .Setup(repo => repo.DeleteMealAsync(mealId))
            .ReturnsAsync(0);

        var mealsController = new MealsController(mockMealService.Object, mockAuthService.Object);

        // Act
        var result = await mealsController.Delete(mealId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}