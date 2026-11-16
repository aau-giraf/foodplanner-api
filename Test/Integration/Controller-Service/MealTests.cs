
using FoodplannerApi.Controller;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Integration.Controllers;

public class MealsTests
{
    private readonly Mock<IMealsRepository> mockRepository = new();
    private readonly Mock<IPackedRepository> mockPackedRepository = new();
    private readonly Mock<IIngredientsRepository> mockIngredientsRepository = new();
    private readonly Mock<IAuthService> mockAuthService = new();
    private readonly MealsController controller;

    public MealsTests()
    {
        var service = new MealsService(
            mockRepository.Object,
            mockPackedRepository.Object,
            mockIngredientsRepository.Object);

        controller = new MealsController(service, mockAuthService.Object);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllMeals_ShouldReturnCorrectMeals()
    {
        // Arrange
        int mealId1 = 42;
        int mealId2 = 32;
        var expected1 = CreateMeal(mealId1, "Test Meal1", 12345);
        var expected2 = CreateMeal(mealId2, "Test Meal2", 12345);

        var expectedMeals = new[] { expected1, expected2 };

        mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedMeals);

        mockPackedRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        mockIngredientsRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        var result = await controller.GetAllMeals();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var meals = Assert.IsAssignableFrom<IEnumerable<MealWithIngredientsDTO>>(okResult.Value).ToList();

        Assert.Equal(2, meals.Count);
        Assert.Equal(expected1.Id, meals[0].Id);
        Assert.Equal(expected1.Name, meals[0].Name);
        Assert.Equal(expected2.Id, meals[1].Id);
        Assert.Equal(expected2.Name, meals[1].Name);

        Assert.All(meals, meal => Assert.Empty(meal.Ingredients));

        mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        mockPackedRepository.Verify(r => r.GetAllAsync(), Times.Once);
        mockIngredientsRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByUser_ShouldGetAllMealsByUser()
    {
        
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetMeal_ShouldGetMeal()
    {
        
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateAsync_ShouldCreateMeal()
    {
        
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsync_ShouldUpdateMeal()
    {
        
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteAsync_ShouldDeleteMeal()
    {
        
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllTemp_ShouldGetAllTemplates()
    {
        
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsyncTemp_ShouldUpdateTemplate()
    {
        
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Get_ShouldGetUniqueIngredientFromMeal()
    {
        
    }

    private static MealDTO CreateMeal(int id, string name, int userId)
    {
        return new MealDTO
        {
            Id = id,
            Name = name,
            User_id = userId,
            Date = "2026-11-16",
            Template = false,
            Image_id = null
        };
    }
}
