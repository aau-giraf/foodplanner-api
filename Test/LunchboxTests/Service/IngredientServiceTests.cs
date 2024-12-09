using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Moq;

namespace Test.Service;

public class IngredientServiceTests
{
    [Fact]
    public async Task GetAllIngredientsAsync_ReturnsAllIngredients()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var expectedIngredients = new List<Ingredient>
        {
            new Ingredient { Id = 1, Name = "Tomato", User_id = 2 },
            new Ingredient { Id = 2, Name = "Cheese", User_id = 1}
        };

        mockIngredientRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedIngredients);

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act
        var result = await ingredientService.GetAllIngredientsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedIngredients.Count, result.Count());
        Assert.Equal(expectedIngredients, result);
    }

    [Fact]
    public async Task GetAllIngredientsByUserAsync_ReturnsIngredientsForUser()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        int userId = 123;
        var expectedIngredients = new List<Ingredient>
        {
            new Ingredient { Id = 1, Name = "Lettuce", User_id = userId },
            new Ingredient { Id = 2, Name = "Bacon", User_id = userId }
        };

        mockIngredientRepository
            .Setup(repo => repo.GetAllByUserAsync(userId))
            .ReturnsAsync(expectedIngredients);

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act
        var result = await ingredientService.GetAllIngredientsByUserAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedIngredients.Count, result.Count());
        Assert.All(result, ingredient => Assert.Equal(userId, ingredient.User_id));
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsIngredientById()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var expectedIngredient = new Ingredient { Id = 1, Name = "Tomato", User_id = 2 };

        mockIngredientRepository
            .Setup(repo => repo.GetByIdAsync(expectedIngredient.Id))
            .ReturnsAsync(expectedIngredient);

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act
        var result = await ingredientService.GetIngredientByIdAsync(expectedIngredient.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedIngredient.Id, result.Id);
        Assert.Equal(expectedIngredient.Name, result.Name);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsNull_WhenIngredientDoesNotExist()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        int nonExistingId = 999;
        _ = mockIngredientRepository
            .Setup(repo => repo.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Ingredient)null); // Simulate non-existent ingredient

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act
        var result = await ingredientService.GetIngredientByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);  // Should return null as ingredient doesn't exist
    }

[Fact]
    public async Task CreateIngredientAsync_ReturnsNewIngredientId()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var newIngredient = new Ingredient { Id = 3, Name = "Onion", User_id = 2 };
        int newIngredientId = 42;

        mockIngredientRepository
            .Setup(repo => repo.InsertAsync(It.IsAny<IngredientDTO>(), It.IsAny<int>()))
            .ReturnsAsync(newIngredientId);

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act
        var result = await ingredientService.CreateIngredientAsync(new IngredientDTO { Name = "Onion", Food_image_id = 2 }, 2);

        // Assert
        Assert.Equal(newIngredientId, result);
    }

    [Fact]
    public async Task CreateIngredientAsync_ThrowsArgumentException_WhenIngredientNameIsNull()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();
        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => ingredientService.CreateIngredientAsync(new IngredientDTO { Name = null, Food_image_id = 2 }, 2));
        Assert.Equal("Ingredient name cannot be null or empty", exception.Message);
    }

[Fact]
    public async Task UpdateIngredientAsync_UpdatesIngredient_ReturnsNumberOfAffectedRows()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var ingredientToUpdate = new Ingredient { Id = 1, Name = "Updated Tomato", User_id = 2 };
        int rowsAffected = 1;

        mockIngredientRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Ingredient>(), It.IsAny<int>()))
            .ReturnsAsync(rowsAffected);

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act
        var result = await ingredientService.UpdateIngredientAsync(ingredientToUpdate, ingredientToUpdate.Id);

        // Assert
        Assert.Equal(rowsAffected, result);
    }

    [Fact]
    public async Task UpdateIngredientAsync_ThrowsArgumentException_WhenIngredientIdIsInvalid()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();
        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => ingredientService.UpdateIngredientAsync(new Ingredient { Id = -1, Name = "Invalid", User_id = 1 }, -1));
        Assert.Equal("Invalid ingredient ID", exception.Message);
    }

    [Fact]
    public async Task DeleteIngredientAsync_DeletesIngredient_ReturnsNumberOfAffectedRows()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        int ingredientIdToDelete = 1;
        int rowsAffected = 1;

        mockIngredientRepository
            .Setup(repo => repo.DeleteAsync(ingredientIdToDelete))
            .ReturnsAsync(rowsAffected);

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act
        var result = await ingredientService.DeleteIngredientAsync(ingredientIdToDelete);

        // Assert
        Assert.Equal(rowsAffected, result);
    }

    [Fact]
    public async Task DeleteIngredientAsync_ThrowsArgumentException_WhenIngredientIdDoesNotExist()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();
        int nonExistingId = 999;
        mockIngredientRepository
            .Setup(repo => repo.DeleteAsync(nonExistingId))
            .ReturnsAsync(0); // No rows affected for non-existing ID

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => ingredientService.DeleteIngredientAsync(nonExistingId));
        Assert.Equal("Ingredient not found", exception.Message);
    }

}
