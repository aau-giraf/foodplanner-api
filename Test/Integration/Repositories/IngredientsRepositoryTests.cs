using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Npgsql;
using Test.Integration;
using Xunit;

namespace Test.Integration.Repositories;

[Collection("IntegrationDatabase")]
public class IngredientsRepositoryIntegrationTests
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly IngredientsRepository repository;
    private readonly List<int> createdIngredientIds = new();

    public IngredientsRepositoryIntegrationTests(DatabaseFixture fixture)
    {
        connectionFactory = fixture.ConnectionFactory;
        repository = new IngredientsRepository(new DapperExecutor(connectionFactory));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_ShouldReturnInsertedIngredients()
    {
        try
        {
            // Arrange
            int firstId = await InsertTestIngredientAsync("First Ingredient", 12345);
            int secondId = await InsertTestIngredientAsync("Second Ingredient", 12345);

            // Act
            var result = (await repository.GetAllAsync()).ToList();

            // Assert
            var first = Assert.Single(result.Where(i => i.Id == firstId));
            var second = Assert.Single(result.Where(i => i.Id == secondId));
            Assert.Equal("First Ingredient", first.Name);
            Assert.Equal("Second Ingredient", second.Name);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByUserAsync_ShouldReturnOnlyMatchingIngredients()
    {
        try
        {
            // Arrange
            int userId = 12345;
            int matchingId = await InsertTestIngredientAsync("User Ingredient", userId);
            int otherId = await InsertTestIngredientAsync("Other User Ingredient", 67890);

            // Act
            var result = (await repository.GetAllByUserAsync(userId)).ToList();

            // Assert
            Assert.Contains(result, i => i.Id == matchingId);
            Assert.DoesNotContain(result, i => i.Id == otherId);
            Assert.All(result, i => Assert.Equal(userId, i.User_id));
        }
        finally
        {
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByUserAsync_ShouldReturnEmptyForUnknownUser()
    {
        // A user ID outside the positive IDs used by the application.
        var result = await repository.GetAllByUserAsync(-1);
        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_ShouldReturnCorrectIngredient()
    {
        try
        {
            // Arrange
            int id = await InsertTestIngredientAsync("Test Ingredient", 12345);

            // Act
            var result = await repository.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Test Ingredient", result.Name);
            Assert.Equal(12345, result.User_id);
            Assert.Null(result.Image_id);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_ShouldReturnNullForMissingIngredient()
    {
        // Identity-generated ingredient IDs are positive.
        var result = await repository.GetByIdAsync(-1);
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InsertAsync_ShouldPersistCorrectIngredientData()
    {
        try
        {
            // Arrange
            var ingredient = new IngredientDTO
            {
                Id = 0,
                Name = "Inserted Ingredient",
                User_id = 12345,
                Image_id = null
            };

            // Act
            int id = await repository.InsertAsync(ingredient);
            createdIngredientIds.Add(id);

            // Assert
            Assert.True(id > 0);
            var stored = await GetStoredIngredientAsync(id);
            Assert.NotNull(stored);
            Assert.Equal(id, stored.Value.Id);
            Assert.Equal(ingredient.Name, stored.Value.Name);
            Assert.Equal(ingredient.User_id, stored.Value.UserId);
            Assert.Null(stored.Value.ImageId);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsync_ShouldPersistCorrectIngredientData()
    {
        try
        {
            // Arrange
            int id = await InsertTestIngredientAsync("Original Ingredient", 12345);
            int otherId = await InsertTestIngredientAsync("Unchanged Ingredient", 12345);
            var updated = new IngredientDTO
            {
                Id = id,
                Name = "Updated Ingredient",
                User_id = 67890,
                Image_id = null
            };

            // Act
            int affectedRows = await repository.UpdateAsync(updated);

            // Assert
            Assert.Equal(1, affectedRows);
            var stored = await GetStoredIngredientAsync(id);
            Assert.NotNull(stored);
            Assert.Equal(id, stored.Value.Id);
            Assert.Equal("Updated Ingredient", stored.Value.Name);
            Assert.Equal(67890, stored.Value.UserId);
            Assert.Null(stored.Value.ImageId);

            var unchanged = await GetStoredIngredientAsync(otherId);
            Assert.NotNull(unchanged);
            Assert.Equal("Unchanged Ingredient", unchanged.Value.Name);
            Assert.Equal(12345, unchanged.Value.UserId);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteAsync_ShouldRemoveOnlySpecifiedIngredient()
    {
        try
        {
            // Arrange
            int deletedId = await InsertTestIngredientAsync("Ingredient To Delete", 12345);
            int keptId = await InsertTestIngredientAsync("Ingredient To Keep", 12345);

            // Act
            int affectedRows = await repository.DeleteAsync(deletedId);

            // Assert
            Assert.Equal(1, affectedRows);
            Assert.Null(await GetStoredIngredientAsync(deletedId));
            var remaining = await GetStoredIngredientAsync(keptId);
            Assert.NotNull(remaining);
            Assert.Equal("Ingredient To Keep", remaining.Value.Name);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    // Set up test data directly in PostgreSQL to test repository methods independently.
    private async Task<int> InsertTestIngredientAsync(string name, int userId)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "INSERT INTO ingredients (name, user_id, image_id) " +
            "VALUES (@name, @userId, NULL) RETURNING id",
            connection);

        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("userId", userId);

        int id = (int)(await command.ExecuteScalarAsync())!;
        createdIngredientIds.Add(id);
        return id;
    }

    private async Task<(int Id, string Name, int UserId, int? ImageId)?>
        GetStoredIngredientAsync(int id)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT id, name, user_id, image_id FROM ingredients WHERE id = @id",
            connection);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return (
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetInt32(2),
            reader.IsDBNull(3) ? null : reader.GetInt32(3)
        );
    }

    private async Task CleanupAsync()
    {
        if (createdIngredientIds.Count == 0)
            return;

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "DELETE FROM ingredients WHERE id = ANY(@ids)",
            connection);
        command.Parameters.AddWithValue("ids", createdIngredientIds.ToArray());
        await command.ExecuteNonQueryAsync();
        createdIngredientIds.Clear();
    }
}
