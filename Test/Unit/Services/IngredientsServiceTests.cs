using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;
using FoodplannerModels.Tags;

namespace Test.Unit.Service;

public class IngredientsServiceTests
{
    // Mock repositories and service
    private readonly Mock<IIngredientsRepository> mockIngredientsRepository = new();
    private readonly IngredientsService ingredientsService;
    public IngredientsServiceTests()
    {
        ingredientsService = new IngredientsService(
            mockIngredientsRepository.Object
        );
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllIngredientsByUserAsync_Success()
    {
        // Arrange
        int userId = 1;
        var ingredients = new List<Ingredient> {
            new Ingredient{ Id = 1, Name = "Apple", User_id = 1},
            new Ingredient{ Id = 2, Name = "Tomato", User_id = 1}
        };
        mockIngredientsRepository.Setup(repo => repo.GetAllByUserAsync(userId)).ReturnsAsync(ingredients);

        // Act
        var result = await ingredientsService.GetAllIngredientsByUserAsync(userId);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count());
        Assert.Equal("Apple", resultList[0].Name);
        Assert.Equal("Tomato", resultList[1].Name);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllIngredientsByUserAsync_RepositoryError()
    {
        // Arrange
        int userId = 1;
        mockIngredientsRepository.Setup(repo => repo.GetAllByUserAsync(userId)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => ingredientsService.GetAllIngredientsByUserAsync(userId));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetIngredientByIdAsync_Success()
    {
        // Arrange
        int ingredientId = 1;
        var ingredient = new Ingredient
        {
            Id = ingredientId,
            Name = "Tomato",
            User_id = 2
        };
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(ingredientId)).ReturnsAsync(ingredient);

        // Act
        var result = await ingredientsService.GetIngredientByIdAsync(ingredientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ingredientId, result.Id);
        Assert.Equal("Tomato", result.Name);
        Assert.Equal(2, result.User_id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetIngredientByIdAsync_IngredientDoesNotExist()
    {
        // Arrange
        int nonExistingId = 999;
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(nonExistingId)).ReturnsAsync((Ingredient?)null);

        // Act
        var result = await ingredientsService.GetIngredientByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetIngredientByIdAsync_RepositoryError()
    {
        // Arrange
        int id = 1;
        mockIngredientsRepository.Setup(repo => repo.GetByIdAsync(id)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => ingredientsService.GetIngredientByIdAsync(id));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreateIngredientAsync_Success()
    {
        // Arrange
        int expectedIngredientId = 42;
        var ingredient = new Ingredient
        {
            Id = 0,
            Name = "Onion",
            User_id = 0
        };
        IEnumerable<int> categoryIds = new List<int>() {1};

        mockIngredientsRepository.Setup(repo => repo.InsertAsync(It.IsAny<Ingredient>(), It.IsAny<IEnumerable<int>>())).ReturnsAsync(expectedIngredientId);

        // Act
        var result = await ingredientsService.CreateIngredientAsync(ingredient, categoryIds);

        // Assert
        Assert.Equal(expectedIngredientId, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreateIngredientAsync_RepositoryError()
    {
        // Arrange
        var ingredient = new Ingredient
        {
            Id = 0,
            Name = "Onion",
            User_id = 0
        };

        IEnumerable<int> categoryIds = new List<int>() {1};

        mockIngredientsRepository.Setup(repo => repo.InsertAsync(ingredient, categoryIds)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => ingredientsService.CreateIngredientAsync(ingredient, categoryIds));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateIngredientAsync_Success()
    {
        // Arrange
        int expectedResult = 1;
        var ingredient = new Ingredient { Id = 1, Name = "Updated Tomato", User_id = 2 };
        mockIngredientsRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Ingredient>())).ReturnsAsync(expectedResult);

        // Act
        var result = await ingredientsService.UpdateIngredientAsync(ingredient);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateIngredientAsync_RepositoryError()
    {
        // Arrange
        var ingredient = new Ingredient
        {
            Id = 0,
            Name = "Onion",
            User_id = 0
        };
        mockIngredientsRepository.Setup(repo => repo.UpdateAsync(ingredient)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => ingredientsService.UpdateIngredientAsync(ingredient));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteIngredientAsync_Success()
    {
        // Arrange
        int expectedResult = 1;
        int ingredientIdToDelete = 1;
        mockIngredientsRepository.Setup(repo => repo.DeleteAsync(ingredientIdToDelete)).ReturnsAsync(expectedResult);

        // Act
        var result = await ingredientsService.DeleteIngredientAsync(ingredientIdToDelete);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteIngredientAsync_RepositoryError()
    {
        // Arrange
        var ingredientIdToDelete = 1;
        mockIngredientsRepository.Setup(repo => repo.DeleteAsync(ingredientIdToDelete)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => ingredientsService.DeleteIngredientAsync(ingredientIdToDelete));
    }
}