using FoodplannerApi.Controller;
using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;
using Xunit;

namespace Test.RepositoryTests;

public class PackedRepositoryTests
{
    private readonly Mock<IDapperExecutor> mockExecutor = new();
    private readonly PackedRepository repository;

    public PackedRepositoryTests()
    {
        repository = new PackedRepository(mockExecutor.Object);
    }

    private static bool HasParameter(object? parameters, string name, object? expectedValue)
    {
        if (parameters == null)
            return false;

        var property = parameters.GetType().GetProperty(name);

        if (property == null)
            return false;

        return Equals(property.GetValue(parameters), expectedValue);
    }

    [Fact]
    public async Task GetAllAsync_Success()
    {
        // Arrange
        var packedIngredients = new List<PackedDTO>
        {
            new() { Meal_id = 1, Ingredient_id = 2, order_no = 1 },
            new() { Meal_id = 1, Ingredient_id = 3, order_no = 2 }
        };

        const string sql = "SELECT * FROM packed";

        mockExecutor
            .Setup(e => e.QueryAsync<PackedDTO>(sql, It.IsAny<object?>()))
            .ReturnsAsync(packedIngredients);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Meal_id);
        Assert.Equal(2, result[0].Ingredient_id);
        Assert.Equal(1, result[0].order_no);

        Assert.Equal(1, result[1].Meal_id);
        Assert.Equal(3, result[1].Ingredient_id);
        Assert.Equal(2, result[1].order_no);

        mockExecutor.Verify(
            e => e.QueryAsync<PackedDTO>(sql, null),
            Times.Once);
    }

    [Fact]
    public async Task GetAllByMealIdAsync_Success()
    {
        // Arrange
        int mealId = 1;

        var packedIngredients = new List<PackedDTO>
        {
            new() { Meal_id = mealId, Ingredient_id = 2, order_no = 1 },
            new() { Meal_id = mealId, Ingredient_id = 3, order_no = 2 }
        };

        const string sql = "SELECT * FROM packed WHERE meal_id = @Id ORDER BY order_no";

        mockExecutor
            .Setup(e => e.QueryAsync<PackedDTO>(
                sql,
                It.Is<object?>(p => HasParameter(p, "Id", mealId))))
            .ReturnsAsync(packedIngredients);

        // Act
        var result = (await repository.GetAllByMealIdAsync(mealId)).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.All(result, item => Assert.Equal(mealId, item.Meal_id));

        Assert.Equal(2, result[0].Ingredient_id);
        Assert.Equal(1, result[0].order_no);

        Assert.Equal(3, result[1].Ingredient_id);
        Assert.Equal(2, result[1].order_no);

        mockExecutor.Verify(
            e => e.QueryAsync<PackedDTO>(
                sql,
                It.Is<object?>(p => HasParameter(p, "Id", mealId))),
            Times.Once);
    }

    [Fact]
    public async Task InsertAsync_Success()
    {
        // Arrange
        var packedIngredient = new PackedDTO
        {
            Meal_id = 1,
            Ingredient_id = 2,
            order_no = 3
        };

        const string sql = "INSERT INTO packed (meal_id, ingredient_id, order_no) VALUES (@MealId, @IngredientId, @OrderNumber)";

        mockExecutor
            .Setup(e => e.ExecuteAsync(
                sql,
                It.Is<object?>(p =>
                    HasParameter(p, "MealId", packedIngredient.Meal_id) &&
                    HasParameter(p, "IngredientId", packedIngredient.Ingredient_id) &&
                    HasParameter(p, "OrderNumber", packedIngredient.order_no))))
            .ReturnsAsync(1);

        // Act
        var result = await repository.InsertAsync(packedIngredient);

        // Assert
        Assert.Equal(1, result);

        mockExecutor.Verify(
            e => e.ExecuteAsync(sql, It.IsAny<object?>()),
            Times.Once);
    }
    [Fact]
    public async Task InsertAsync_NoRowsAffected()
    {
        // Arrange
        var packedIngredient = new PackedDTO
        {
            Meal_id = 1,
            Ingredient_id = 2,
            order_no = 3
        };

        const string sql = "INSERT INTO packed (meal_id, ingredient_id, order_no) VALUES (@MealId, @IngredientId, @OrderNumber)";

        mockExecutor
            .Setup(e => e.ExecuteAsync(sql, It.IsAny<object?>()))
            .ReturnsAsync(0);

        // Act
        var result = await repository.InsertAsync(packedIngredient);

        // Assert
        Assert.Equal(0, result);

    }

    [Fact]
    public async Task UpdateAsync_Success()
    {
        // Arrange
        var oldPacked = new PackedDTO
        {
            Meal_id = 1,
            Ingredient_id = 2,
            order_no = 1
        };

        var newPacked = new PackedDTO
        {
            Meal_id = 1,
            Ingredient_id = 3,
            order_no = 2
        };

        const string sql = "UPDATE packed SET meal_id = @NewMealId, ingredient_id = @NewIngredientId, order_no = @NewOrderNumber WHERE meal_id = @OldMealId AND ingredient_id = @OldIngredientId";

        mockExecutor
            .Setup(e => e.ExecuteAsync(
                sql,
                It.Is<object?>(p =>
                    HasParameter(p, "NewMealId", newPacked.Meal_id) &&
                    HasParameter(p, "NewIngredientId", newPacked.Ingredient_id) &&
                    HasParameter(p, "NewOrderNumber", newPacked.order_no) &&
                    HasParameter(p, "OldMealId", oldPacked.Meal_id) &&
                    HasParameter(p, "OldIngredientId", oldPacked.Ingredient_id))))
            .ReturnsAsync(1);

        // Act
        var result = await repository.UpdateAsync(oldPacked, newPacked);

        // Assert
        Assert.Equal(1, result);

        mockExecutor.Verify(
            e => e.ExecuteAsync(sql, It.IsAny<object?>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NoRowsAffected()
    {
        // Arrange
        var oldPacked = new PackedDTO
        {
            Meal_id = 1,
            Ingredient_id = 2,
            order_no = 1
        };

        var newPacked = new PackedDTO
        {
            Meal_id = 1,
            Ingredient_id = 3,
            order_no = 2
        };

        const string sql = "UPDATE packed SET meal_id = @NewMealId, ingredient_id = @NewIngredientId, order_no = @NewOrderNumber WHERE meal_id = @OldMealId AND ingredient_id = @OldIngredientId";

        mockExecutor
            .Setup(e => e.ExecuteAsync(sql, It.IsAny<object?>()))
            .ReturnsAsync(0);

        // Act
        var result = await repository.UpdateAsync(oldPacked, newPacked);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task DeleteAsync_Success()
    {
        // Arrange
        var packedIngredient = new PackedDTO
        {
            Meal_id = 1,
            Ingredient_id = 2,
            order_no = 1
        };

        const string sql = "DELETE FROM packed WHERE meal_id = @MealId AND ingredient_id = @IngredientId";

        mockExecutor
            .Setup(e => e.ExecuteAsync(
                sql,
                It.Is<object?>(p =>
                    HasParameter(p, "MealId", packedIngredient.Meal_id) &&
                    HasParameter(p, "IngredientId", packedIngredient.Ingredient_id))))
            .ReturnsAsync(1);

        // Act
        var result = await repository.DeleteAsync(packedIngredient);

        // Assert
        Assert.Equal(1, result);

        mockExecutor.Verify(
            e => e.ExecuteAsync(sql, It.IsAny<object?>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NoRowsAffected()
    {
        // Arrange
        var packedIngredient = new PackedDTO
        {
            Meal_id = 1,
            Ingredient_id = 2,
            order_no = 1
        };

        const string sql = "DELETE FROM packed WHERE meal_id = @MealId AND ingredient_id = @IngredientId";

        mockExecutor
            .Setup(e => e.ExecuteAsync(sql, It.IsAny<object?>()))
            .ReturnsAsync(0);

        // Act
        var result = await repository.DeleteAsync(packedIngredient);

        // Assert
        Assert.Equal(0, result);
    }
}