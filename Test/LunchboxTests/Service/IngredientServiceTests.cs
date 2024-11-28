using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Moq;
using Xunit;

public class IngredientServiceTests
{
    [Fact]
    public async Task GetAllIngredientsAsync_ReturnsAllIngredients()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var expectedIngredients = new List<Ingredient>
        {
            new Ingredient { Id = 1, Name = "Tomato", User_ref = 2 },
            new Ingredient { Id = 2, Name = "Cheese", User_ref = 1}
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
            new Ingredient { Id = 1, Name = "Lettuce", User_ref = userId },
            new Ingredient { Id = 2, Name = "Bacon", User_ref = userId }
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
        Assert.All(result, ingredient => Assert.Equal(userId, ingredient.User_ref));
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsIngredientById()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var expectedIngredient = new Ingredient { Id = 1, Name = "Tomato", User_ref = 2 };

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
    public async Task CreateIngredientAsync_ReturnsNewIngredientId()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var newIngredient = new Ingredient { Id = 3, Name = "Onion", User_ref = 2 };
        int newIngredientId = 42;

        mockIngredientRepository
            .Setup(repo => repo.InsertAsync(newIngredient))
            .ReturnsAsync(newIngredientId);

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act
        var result = await ingredientService.CreateIngredientAsync(newIngredient);

        // Assert
        Assert.Equal(newIngredientId, result);
    }

    [Fact]
    public async Task UpdateIngredientAsync_UpdatesIngredient_ReturnsNumberOfAffectedRows()
    {
        // Arrange
        var mockIngredientRepository = new Mock<IIngredientRepository>();

        var ingredientToUpdate = new Ingredient { Id = 1, Name = "Updated Tomato", User_ref = 2 };
        int rowsAffected = 1;

        mockIngredientRepository
            .Setup(repo => repo.UpdateAsync(ingredientToUpdate, ingredientToUpdate.Id))
            .ReturnsAsync(rowsAffected);

        var ingredientService = new IngredientService(mockIngredientRepository.Object);

        // Act
        var result = await ingredientService.UpdateIngredientAsync(ingredientToUpdate, ingredientToUpdate.Id);

        // Assert
        Assert.Equal(rowsAffected, result);
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
}
