using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Moq;
using AutoMapper;

namespace Test.Service;

public class MealServiceTests
{
    private readonly Mock<IMealRepository> _mockMealRepository;
    private readonly Mock<IPackedIngredientRepository> _mockPackedIngredientRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MealService _mealService;

    public MealServiceTests()
    {
        _mockMealRepository = new Mock<IMealRepository>();
        _mockPackedIngredientRepository = new Mock<IPackedIngredientRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();

        _mealService = new MealService(
            _mockMealRepository.Object,
            _mockPackedIngredientRepository.Object,
            _mockIngredientRepository.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetAllMealsAsync_ReturnsMeals_WhenMealsExist()
    {
        // Arrange
        var expectedMeals = new List<Meal>
        {
            new Meal { Id = 1, Name = "Pizza", Date = "18/11/2022" },
            new Meal { Id = 2, Name = "Burger", Date = "19/01/2013" }
        };

        _mockMealRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedMeals);

        _mockPackedIngredientRepository.Setup(repo => repo.GetAllByMealIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<PackedIngredient>());

        var ingredients = new List<Ingredient>
        {
            new Ingredient { Id = 1, Name = "Cheese", User_id = 2 },
            new Ingredient { Id = 2, Name = "Tomato", User_id = 1 }
        };

        // Mock the ingredient retrieval
        _mockIngredientRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => ingredients.FirstOrDefault(i => i.Id == id));

        // Act
        var result = await _mealService.GetAllMealsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMeals.Count, result.Count());
        Assert.All(result, meal => Assert.Contains(expectedMeals, m => m.Id == meal.Id && m.Name == meal.Name));
    }

    [Fact]
    public async Task GetMealByIdAsync_ReturnsMeal_WhenMealExists()
    {
        // Arrange
        var expectedMeal = new Meal { Id = 1, Name = "Pasta", Date = "20/03/2022" };
        _mockMealRepository.Setup(repo => repo.GetByIdAsync(expectedMeal.Id))
            .ReturnsAsync(expectedMeal);

        _mockPackedIngredientRepository.Setup(repo => repo.GetAllByMealIdAsync(expectedMeal.Id))
            .ReturnsAsync(new List<PackedIngredient>());

        var ingredient = new Ingredient { Id = 1, Name = "Noodles", User_id = 3 };
        _mockIngredientRepository.Setup(repo => repo.GetByIdAsync(ingredient.Id))
            .ReturnsAsync(ingredient);

        // Act
        var result = await _mealService.GetMealByIdAsync(expectedMeal.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMeal.Id, result.Id);
        Assert.Equal(expectedMeal.Name, result.Name);
    }

    [Fact]
    public async Task GetMealByIdAsync_ReturnsNull_WhenMealDoesNotExist()
    {
        // Arrange
        int nonExistingId = 999;
        _mockMealRepository.Setup(repo => repo.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Meal)null);

        // Act
        var result = await _mealService.GetMealByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateMealAsync_ReturnsNewMealId()
    {
        // Arrange
        var mealCreateDTO = new MealCreateDTO { Name = "Salad", Date = "21/03/2023" };
        var newMealId = 42;

        _mockMapper.Setup(m => m.Map<Meal>(It.IsAny<MealCreateDTO>()))
            .Returns(new Meal { Id = newMealId, Name = "Salad", Date = "21/03/2023" });

        _mockMealRepository.Setup(repo => repo.InsertAsync(It.IsAny<Meal>(), It.IsAny<int>()))
            .ReturnsAsync(newMealId);

        // Act
        var result = await _mealService.CreateMealAsync(mealCreateDTO, 1);

        // Assert
        Assert.Equal(newMealId, result);
    }

    [Fact]
    public async Task UpdateMealAsync_ReturnsNumberOfAffectedRows()
    {
        // Arrange
        var mealToUpdate = new Meal { Id = 1, Name = "Updated Salad", Date = "22/03/2023" };
        int rowsAffected = 1;

        _mockMealRepository.Setup(repo => repo.UpdateAsync(mealToUpdate, mealToUpdate.Id))
            .ReturnsAsync(rowsAffected);

        // Act
        var result = await _mealService.UpdateMealAsync(mealToUpdate, mealToUpdate.Id);

        // Assert
        Assert.Equal(rowsAffected, result);
    }

    [Fact]
    public async Task DeleteMealAsync_ReturnsNumberOfAffectedRows()
    {
        // Arrange
        int mealIdToDelete = 1;
        int rowsAffected = 1;

        _mockMealRepository.Setup(repo => repo.DeleteAsync(mealIdToDelete))
            .ReturnsAsync(rowsAffected);

        // Act
        var result = await _mealService.DeleteMealAsync(mealIdToDelete);

        // Assert
        Assert.Equal(rowsAffected, result);
    }
}