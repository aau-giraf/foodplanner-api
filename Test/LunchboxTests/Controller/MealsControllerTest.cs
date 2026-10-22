using FoodplannerApi.Controller;
using FoodplannerModels.Auth;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test.LunchboxTests.Controller;

public class MealsControllerTests
{
    private readonly Mock<IMealsService> mockMealsService = new();
    private readonly Mock<IAuthService> mockAuthService = new();
    private readonly MealsController mealsController;

    public MealsControllerTests()
    {
        mealsController = new MealsController(mockMealsService.Object, mockAuthService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var meals = new List<MealWithIngredientsDTO>
        {
            new() { Id = 1, Name = "Pizza", Date = "18/11/2022", Template = false, Image_id = null, User_id = 1, Ingredients = [new IngredientDTO { Id = 10, Name = "Cheese", User_id = 1 }] },
            new() { Id = 2, Name = "Burger", Date = "19/01/2013", Template = true, Image_id = null, User_id = 2, Ingredients = [] }
        };
        mockMealsService.Setup(service => service.GetAllMealsAsync()).ReturnsAsync(meals);

        // Act
        var result = await mealsController.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMeals = Assert.IsAssignableFrom<IEnumerable<MealWithIngredientsDTO>>(okResult.Value).ToList();
        Assert.Equal(2, returnedMeals.Count);
        Assert.Equal(1, returnedMeals[0].Id);
        Assert.Equal("Pizza", returnedMeals[0].Name);
        Assert.False(returnedMeals[0].Template);
        Assert.Single(returnedMeals[0].Ingredients);
        Assert.Equal("Cheese", returnedMeals[0].Ingredients[0].Name);
        Assert.Equal(2, returnedMeals[1].Id);
        Assert.Equal("Burger", returnedMeals[1].Name);
        Assert.True(returnedMeals[1].Template);
    }

    [Fact]
    public async Task GetAllByUser_ReturnsOkObjectResult()
    {
        // Arrange
        const string token = "Bearer TestToken";
        const string date = "18/11/2022";
        int userId = 1;
        var meals = new List<MealWithIngredientsDTO>
        {
            new() { Id = 1, Name = "Pizza", Date = date, Template = false, Image_id = null, User_id = userId, Ingredients = [] }
        };
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());
        mockMealsService.Setup(service => service.GetAllMealsByUserAndDateAsync(userId, date)).ReturnsAsync(meals);

        // Act
        var result = await mealsController.GetAllByUser(token, date);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMeals = Assert.IsAssignableFrom<IEnumerable<MealWithIngredientsDTO>>(okResult.Value).ToList();
        Assert.Single(returnedMeals);
        Assert.Equal(1, returnedMeals[0].Id);
        Assert.Equal("Pizza", returnedMeals[0].Name);
        Assert.Equal(date, returnedMeals[0].Date);
        Assert.Equal(userId, returnedMeals[0].User_id);
        mockMealsService.Verify(service => service.GetAllMealsByUserAndDateAsync(userId, date), Times.Once);
    }

    [Fact]
    public async Task TeacherGetUserMeals_ReturnsOkObjectResult()
    {
        // Arrange
        int userId = 1;
        const string date = "18/11/2022";
        var meals = new List<MealWithIngredientsDTO>
        {
            new() { Id = 3, Name = "Salad", Date = date, Template = false, Image_id = null, User_id = userId, Ingredients = [] }
        };
        mockMealsService.Setup(service => service.GetAllMealsByUserAndDateAsync(userId, date)).ReturnsAsync(meals);

        // Act
        var result = await mealsController.TeacherGetUserMeals(date, userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMeals = Assert.IsAssignableFrom<IEnumerable<MealWithIngredientsDTO>>(okResult.Value).ToList();
        Assert.Single(returnedMeals);
        Assert.Equal(3, returnedMeals[0].Id);
        Assert.Equal("Salad", returnedMeals[0].Name);
        Assert.Equal(userId, returnedMeals[0].User_id);
        mockMealsService.Verify(service => service.GetAllMealsByUserAndDateAsync(userId, date), Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsOkObjectResult_WhenMealDoesExist()
    {
        // Arrange
        int mealId = 1;
        var meal = new MealWithIngredientsDTO { Id = mealId, Name = "Pizza", Date = "18/11/2022", Template = false, Image_id = null, User_id = 1, Ingredients = [new IngredientDTO { Id = 10, Name = "Cheese", User_id = 1 }] };
        mockMealsService.Setup(service => service.GetMealByIdAsync(mealId)).ReturnsAsync(meal);

        // Act
        var result = await mealsController.Get(mealId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMeal = Assert.IsType<MealWithIngredientsDTO>(okResult.Value);
        Assert.Equal(mealId, returnedMeal.Id);
        Assert.Equal("Pizza", returnedMeal.Name);
        Assert.Equal("18/11/2022", returnedMeal.Date);
        Assert.False(returnedMeal.Template);
        Assert.Single(returnedMeal.Ingredients);
        Assert.Equal("Cheese", returnedMeal.Ingredients[0].Name);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundResult_WhenMealDoesNotExist()
    {
        // Arrange
        int mealId = 999;
        mockMealsService.Setup(service => service.GetMealByIdAsync(mealId))
            .ReturnsAsync((MealWithIngredientsDTO?)null);

        // Act
        var result = await mealsController.Get(mealId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockMealsService.Verify(service => service.GetMealByIdAsync(mealId), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WhenMealIsCreated()
    {
        // Arrange
        const string token = "Bearer TestToken";
        int userId = 1;
        int mealId = 42;
        var mealCreateDTO = new MealCreateDTO { Id = "1",Name = "Pizza", Date = "18/11/2022", Template = false, Image_id = null };
        var createdMeal = new MealWithIngredientsDTO { Id = mealId, Name = "Pizza", Date = "18/11/2022", Template = false, Image_id = null, User_id = userId, Ingredients = [] };
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());
        mockMealsService.Setup(service => service.CreateMealAsync(mealCreateDTO, userId)).ReturnsAsync(mealId);
        mockMealsService.Setup(service => service.GetMealByIdAsync(mealId)).ReturnsAsync(createdMeal);

        // Act
        var result = await mealsController.Create(token, mealCreateDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(MealsController.Get), createdResult.ActionName);
        Assert.Equal(mealId, createdResult.RouteValues!["id"]);
        var returnedMeal = Assert.IsType<MealWithIngredientsDTO>(createdResult.Value);
        Assert.Equal(mealId, returnedMeal.Id);
        Assert.Equal("Pizza", returnedMeal.Name);
        Assert.Equal(userId, returnedMeal.User_id);
        mockMealsService.Verify(service => service.CreateMealAsync(mealCreateDTO, userId), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenMealIsNotCreated()
    {
        // Arrange
        const string token = "Bearer TestToken";
        int userId = 1;
        var mealCreateDTO = new MealCreateDTO { Id = "1", Name = "Pizza", Date = "18/11/2022", Template = false, Image_id = null };
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());
        mockMealsService.Setup(service => service.CreateMealAsync(mealCreateDTO, userId)).ReturnsAsync(0);

        // Act
        var result = await mealsController.Create(token, mealCreateDTO);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        mockMealsService.Verify(service => service.CreateMealAsync(mealCreateDTO, userId), Times.Once);
        mockMealsService.Verify(service => service.GetMealByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult_WhenMealIsUpdated()
    {
        // Arrange
        const string token = "Bearer TestToken";
        int mealId = 1;
        int userId = 1;
        var meal = new MealDTO { Id = mealId, Name = "Updated Pizza", Date = "18/11/2022", Template = false, Image_id = null, User_id = userId };
        var updatedMeal = new MealWithIngredientsDTO { Id = mealId, Name = "Updated Pizza", Date = "18/11/2022", Template = false, Image_id = null, User_id = userId, Ingredients = [] };
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());
        mockMealsService.Setup(service => service.UpdateMealAsync(meal)).ReturnsAsync(1);
        mockMealsService.Setup(service => service.GetMealByIdAsync(mealId)).ReturnsAsync(updatedMeal);

        // Act
        var result = await mealsController.Update(token, meal);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMeal = Assert.IsType<MealWithIngredientsDTO>(okResult.Value);
        Assert.Equal(mealId, returnedMeal.Id);
        Assert.Equal("Updated Pizza", returnedMeal.Name);
        Assert.Equal(userId, returnedMeal.User_id);
        mockMealsService.Verify(service => service.UpdateMealAsync(meal), Times.Once);
        mockMealsService.Verify(service => service.GetMealByIdAsync(mealId), Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenMealIsNotUpdated()
    {
        // Arrange
        const string token = "Bearer TestToken";
        int mealId = 1;
        var meal = new MealDTO { Id = mealId, Name = "Pizza", Date = "18/11/2022", Template = false, Image_id = null, User_id = 1 };
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns("1");
        mockMealsService.Setup(service => service.UpdateMealAsync(meal)).ReturnsAsync(0);

        // Act
        var result = await mealsController.Update(token, meal);

        // Assert
        Assert.IsType<BadRequestResult>(result);
        mockMealsService.Verify(service => service.UpdateMealAsync(meal), Times.Once);
        mockMealsService.Verify(service => service.GetMealByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ReturnsOkObjectResult_WhenMealExist()
    {
        // Arrange
        int mealId = 1;
        var meal = new MealWithIngredientsDTO { Id = mealId, Name = "Pizza", Date = "18/11/2022", Template = false, Image_id = null, User_id = 1, Ingredients = [] };
        mockMealsService.Setup(service => service.GetMealByIdAsync(mealId)).ReturnsAsync(meal);
        mockMealsService.Setup(service => service.DeleteMealAsync(mealId)).ReturnsAsync(1);

        // Act
        var result = await mealsController.Delete(mealId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMeal = Assert.IsType<MealWithIngredientsDTO>(okResult.Value);
        Assert.Equal(mealId, returnedMeal.Id);
        Assert.Equal("Pizza", returnedMeal.Name);
        mockMealsService.Verify(service => service.DeleteMealAsync(mealId), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundResult_WhenMealDoesNotExist()
    {
        // Arrange
        int mealId = 999;
        mockMealsService.Setup(service => service.GetMealByIdAsync(mealId))
            .ReturnsAsync((MealWithIngredientsDTO?)null);
        mockMealsService.Setup(service => service.DeleteMealAsync(mealId)).ReturnsAsync(0);

        // Act
        var result = await mealsController.Delete(mealId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockMealsService.Verify(service => service.GetMealByIdAsync(mealId), Times.Once);
        mockMealsService.Verify(service => service.DeleteMealAsync(mealId), Times.Once);
    }
}