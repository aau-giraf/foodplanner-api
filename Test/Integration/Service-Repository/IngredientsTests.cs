using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Moq;

namespace Test.Integration.Services;

public class IngredientsTests
{
    private readonly Mock<IDapperExecutor> mockExecutor = new();
    private readonly IngredientsService service;

    public IngredientsTests()
    {
        var repository = new IngredientsRepository(mockExecutor.Object);
        service = new IngredientsService(repository);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllIngredientsAsync_ShouldReturnIngredients()
    {
        // Arrange
        var ingredients = new[]
        {
            CreateIngredient(1, "First Ingredient", 12345),
            CreateIngredient(2, "Second Ingredient", 12345)
        };

        mockExecutor.Setup(e => e.QueryAsync<IngredientDTO>(
                "SELECT * FROM ingredients", It.IsAny<object?>()))
            .ReturnsAsync(ingredients);

        // Act
        var result = (await service.GetAllIngredientsAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("First Ingredient", result[0].Name);
        Assert.Equal("Second Ingredient", result[1].Name);
        Assert.All(result, ingredient => Assert.Equal(12345, ingredient.User_id));

        mockExecutor.Verify(e => e.QueryAsync<IngredientDTO>(
            "SELECT * FROM ingredients", It.IsAny<object?>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllIngredientsByUserAsync_ShouldReturnOnlyMatchingIngredients()
    {
        // Arrange
        int userId = 12345;
        var expected = CreateIngredient(10, "User Ingredient", userId);

        mockExecutor.Setup(e => e.QueryAsync<IngredientDTO>(
                "SELECT * FROM ingredients WHERE user_id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", userId))))
            .ReturnsAsync(new[] { expected });

        // Act
        var result = (await service.GetAllIngredientsByUserAsync(userId)).ToList();

        // Assert
        var ingredient = Assert.Single(result);
        Assert.Equal(expected.Id, ingredient.Id);
        Assert.Equal(userId, ingredient.User_id);

        mockExecutor.Verify(e => e.QueryAsync<IngredientDTO>(
            "SELECT * FROM ingredients WHERE user_id = @Id",
            It.Is<object?>(p => HasParameter(p, "Id", userId))), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllIngredientsByUserAsync_ShouldReturnEmptyForUnknownUser()
    {
        // Arrange
        int unknownUserId = -1;

        mockExecutor.Setup(e => e.QueryAsync<IngredientDTO>(
                "SELECT * FROM ingredients WHERE user_id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", unknownUserId))))
            .ReturnsAsync(Array.Empty<IngredientDTO>());

        // Act
        var result = await service.GetAllIngredientsByUserAsync(unknownUserId);

        // Assert
        Assert.Empty(result);

        mockExecutor.Verify(e => e.QueryAsync<IngredientDTO>(
            "SELECT * FROM ingredients WHERE user_id = @Id",
            It.Is<object?>(p => HasParameter(p, "Id", unknownUserId))), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetIngredientByIdAsync_ShouldReturnCorrectIngredient()
    {
        // Arrange
        int id = 42;
        var expected = CreateIngredient(id, "Test Ingredient", 12345);

        mockExecutor.Setup(e => e.QuerySingleOrDefaultAsync<IngredientDTO>(
                "SELECT * FROM ingredients WHERE id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", id))))
            .ReturnsAsync(expected);

        // Act
        var result = await service.GetIngredientByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Test Ingredient", result.Name);
        Assert.Equal(12345, result.User_id);
        Assert.Null(result.Image_id);

        mockExecutor.Verify(e => e.QuerySingleOrDefaultAsync<IngredientDTO>(
            "SELECT * FROM ingredients WHERE id = @Id",
            It.Is<object?>(p => HasParameter(p, "Id", id))), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetIngredientByIdAsync_ShouldReturnNullForMissingIngredient()
    {
        // Arrange
        int missingId = -1;

        mockExecutor.Setup(e => e.QuerySingleOrDefaultAsync<IngredientDTO>(
                "SELECT * FROM ingredients WHERE id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", missingId))))
            .ReturnsAsync((IngredientDTO?)null);

        // Act
        var result = await service.GetIngredientByIdAsync(missingId);

        // Assert
        Assert.Null(result);

        mockExecutor.Verify(e => e.QuerySingleOrDefaultAsync<IngredientDTO>(
            "SELECT * FROM ingredients WHERE id = @Id",
            It.Is<object?>(p => HasParameter(p, "Id", missingId))), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateIngredientAsync_ShouldReturnGeneratedId()
    {
        // Arrange
        var ingredient = CreateIngredient(0, "Created Ingredient", 12345);
        int generatedId = 77;

        mockExecutor.Setup(e => e.QuerySingleAsync<int>(
                "INSERT INTO ingredients (name, user_id, image_id) VALUES (@Name, @UserId, @ImageId) RETURNING id",
                It.Is<object?>(p =>
                    HasParameter(p, "Name", ingredient.Name) &&
                    HasParameter(p, "UserId", ingredient.User_id) &&
                    HasParameter(p, "ImageId", DBNull.Value))))
            .ReturnsAsync(generatedId);

        // Act
        int result = await service.CreateIngredientAsync(ingredient);

        // Assert
        Assert.Equal(generatedId, result);

        mockExecutor.Verify(e => e.QuerySingleAsync<int>(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateIngredientAsync_ShouldReturnAffectedRows()
    {
        // Arrange
        var updated = CreateIngredient(42, "Updated Ingredient", 67890);

        mockExecutor.Setup(e => e.ExecuteAsync(
                "UPDATE ingredients SET name = @Name, user_id = @UserId, image_id = @ImageId WHERE id = @Id",
                It.Is<object?>(p =>
                    HasParameter(p, "Id", updated.Id) &&
                    HasParameter(p, "Name", updated.Name) &&
                    HasParameter(p, "UserId", updated.User_id) &&
                    HasParameter(p, "ImageId", null))))
            .ReturnsAsync(1);

        // Act
        int affectedRows = await service.UpdateIngredientAsync(updated);

        // Assert
        Assert.Equal(1, affectedRows);

        mockExecutor.Verify(e => e.ExecuteAsync(
            It.IsAny<string>(), It.IsAny<object?>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteIngredientAsync_ShouldReturnAffectedRows()
    {
        // Arrange
        int ingredientId = 42;

        mockExecutor.Setup(e => e.ExecuteAsync(
                "DELETE FROM ingredients WHERE id = @Id",
                It.Is<object?>(p => HasParameter(p, "Id", ingredientId))))
            .ReturnsAsync(1);

        // Act
        int affectedRows = await service.DeleteIngredientAsync(ingredientId);

        // Assert
        Assert.Equal(1, affectedRows);

        mockExecutor.Verify(e => e.ExecuteAsync(
            "DELETE FROM ingredients WHERE id = @Id",
            It.Is<object?>(p => HasParameter(p, "Id", ingredientId))), Times.Once);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllIngredientsAsync_ShouldWrapRepositoryException()
    {
        // Arrange
        var originalException = new InvalidOperationException("Database operation failed");

        mockExecutor.Setup(e => e.QueryAsync<IngredientDTO>(
                "SELECT * FROM ingredients", It.IsAny<object?>()))
            .ThrowsAsync(originalException);

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.GetAllIngredientsAsync());

        // Assert
        Assert.Contains("Error retrieving all ingredients", exception.Message);
        Assert.Same(originalException, exception.InnerException);
    }

    private static IngredientDTO CreateIngredient(int id, string name, int userId)
    {
        return new IngredientDTO
        {
            Id = id,
            Name = name,
            User_id = userId,
            Image_id = null
        };
    }

    private static bool HasParameter(object? parameters, string name, object? expected)
    {
        if (parameters is null)
            return false;

        var property = parameters.GetType().GetProperty(name);
        return property is not null && Equals(property.GetValue(parameters), expected);
    }
}
