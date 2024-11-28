using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Moq;
using Xunit;

public class MealServiceTests
{
    [Fact]
    public async Task GetAllMealsAsync_ReturnsMeals_WhenMealsExist()
    {
        // Arrange
        var mockMealRepository = new Mock<IMealRepository>();
        var mockPackedIngredientRepository = new Mock<IPackedIngredientRepository>();
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var expectedMeals = new List<Meal>
        {
            new Meal { Id = 1, Title = "Pizza", Date = "18/11/2022" },
            new Meal { Id = 2, Title = "Burger", Date = "19/01/2013" }
        };

        // Mock the repository to return the expected meals
        mockMealRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedMeals);

        // Mock packed ingredient and ingredient repository as they're used in the service
        mockPackedIngredientRepository
            .Setup(repo => repo.GetAllByMealIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<PackedIngredient>());

        mockIngredientRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Ingredient { Id = 1, Name = "Cheese", User_ref = 2 });

        // Create an instance of MealService with the mocked dependencies
        var mealService = new MealService(
            mockMealRepository.Object,
            mockPackedIngredientRepository.Object,
            mockIngredientRepository.Object
        );

        // Act
        var result = await mealService.GetAllMealsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMeals.Count, result.Count());
        Assert.All(result, meal =>
        {
            Assert.Contains(expectedMeals, m => m.Id == meal.Id && m.Title == meal.Title);
        });
    }
}
