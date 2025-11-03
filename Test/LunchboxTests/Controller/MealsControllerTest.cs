using Moq;
using FoodplannerModels.Lunchbox;
using FoodplannerApi.Controller;
using FoodplannerApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using FoodplannerModels.Account;

namespace Test.Controller;

public class MealsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        var meals = new List<MealDTO>
        {
            new() { Id = 1, Name = "Pizza", Date = "test", Ingredients = [] },
            new() { Id = 2, Name = "Burger", Date = "test", Ingredients = [] }
        };

        mockMealService
            .Setup(repo => repo.GetAllMealsAsync())
            .ReturnsAsync(meals);

        var mealsController = new MealsController(mockMealService.Object, authService);

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
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        string date = "test";
        var meals = new List<MealDTO>
        {
            new() { Id = 1, Name = "Pizza", Date = date, Ingredients = [] },
            new() { Id = 2, Name = "Burger", Date = date, Ingredients = [] }
        };
        int userId = 1;
        var user = new User() {Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = "Parent", RoleApproved = true};

        mockMealService
            .Setup(repo => repo.GetAllMealsByUserAsync(userId, date))
            .ReturnsAsync(meals);

        var mealsController = new MealsController(mockMealService.Object, authService);

        // Act
        var result = await mealsController.GetAllByUser("Bearer " + authService.GenerateJWTToken(user), date);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task TeacherGetUserMeals_ReturnsOkObjectResult()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        string date = "test";
        var meals = new List<MealDTO>
        {
            new() { Id = 1, Name = "Pizza", Date = date, Ingredients = [] },
            new() { Id = 2, Name = "Burger", Date = date, Ingredients = [] }
        };
        int userId = 1;
        var user = new User() {Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = "Parent", RoleApproved = true};

        mockMealService
            .Setup(repo => repo.GetAllMealsByUserAsync(userId, date))
            .ReturnsAsync(meals);

        var mealsController = new MealsController(mockMealService.Object, authService);

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
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int mealId = 1;
        var meal = new MealDTO() { Id = mealId, Name = "Pizza", Date = "test", Ingredients = [] };

        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);

        var mealsController = new MealsController(mockMealService.Object, authService);

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
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int mealId = 1;
        MealDTO? meal = null;

        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);

        var mealsController = new MealsController(mockMealService.Object, authService);

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
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int mealId = 1;
        MealCreateDTO mealDTO = new() {Name = "Pizza", Date = "test"};
        MealDTO meal = new() { Id = mealId, Name = "Pizza", Date = "test", Ingredients = [] };
        int userId = 1;
        var user = new User() {Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = "Parent", RoleApproved = true};

        mockMealService
            .Setup(repo => repo.CreateMealAsync(mealDTO, userId))
            .ReturnsAsync(mealId);
        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);

        var mealsController = new MealsController(mockMealService.Object, authService);

        // Act
        var result = await mealsController.Create("Bearer " + authService.GenerateJWTToken(user), mealDTO);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenMealIsNotCreated()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        MealCreateDTO mealDTO = new() {Name = "Pizza", Date = "test"};
        int userId = 1;
        var user = new User() {Id = userId, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = "Parent", RoleApproved = true};

        mockMealService
            .Setup(repo => repo.CreateMealAsync(mealDTO, userId))
            .ReturnsAsync(0);

        var mealsController = new MealsController(mockMealService.Object, authService);

        // Act
        var result = await mealsController.Create("Bearer " + authService.GenerateJWTToken(user), mealDTO);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenMealIsUpdated()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int mealId = 1;
        Meal meal = new() { Id = mealId, Name = "Pizza", Date = "test"};
        MealDTO mealDTO = new() { Id = mealId, Name = "Pizza", Date = "test", Ingredients = [] };
        var user = new User() {Id = 1, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = "Parent", RoleApproved = true};

        mockMealService
            .Setup(repo => repo.UpdateMealAsync(meal, mealId))
            .ReturnsAsync(mealId);
        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(mealDTO);

        var mealsController = new MealsController(mockMealService.Object, authService);

        // Act
        var result = await mealsController.Update("Bearer " + authService.GenerateJWTToken(user), meal, mealId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenMealIsNotUpdated()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int mealId = 1;
        Meal meal = new() { Id = mealId, Name = "Pizza", Date = "test"};
        var user = new User() {Id = 1, FirstName = "test", LastName = "test", Email = "test@example.com", Password = "1234", Role = "Parent", RoleApproved = true};

        mockMealService
            .Setup(repo => repo.UpdateMealAsync(meal, mealId))
            .ReturnsAsync(0);

        var mealsController = new MealsController(mockMealService.Object, authService);

        // Act
        var result = await mealsController.Update("Bearer " + authService.GenerateJWTToken(user), meal, mealId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOkObjectResult_WhenMealExist()
    {
        // Arrange
        var mockMealService = new Mock<IMealService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int mealId = 1;
        MealDTO meal = new() { Id = mealId, Name = "Pizza", Date = "test", Ingredients = [] };

        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);
        mockMealService
            .Setup(repo => repo.DeleteMealAsync(mealId))
            .ReturnsAsync(1);

        var mealsController = new MealsController(mockMealService.Object, authService);

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
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        int mealId = 1;
        MealDTO? meal = null;

        mockMealService
            .Setup(repo => repo.GetMealByIdAsync(mealId))
            .ReturnsAsync(meal);
        mockMealService
            .Setup(repo => repo.DeleteMealAsync(mealId))
            .ReturnsAsync(0);

        var mealsController = new MealsController(mockMealService.Object, authService);

        // Act
        var result = await mealsController.Delete(mealId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}