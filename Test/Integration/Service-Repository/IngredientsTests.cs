using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using FoodplannerServices.Lunchbox;
using Npgsql;

namespace Test.Integration.Services;

[Collection("IntegrationDatabase")]
public class IngredientsTests
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly IngredientsService service;
    private readonly List<int> createdIngredientIds = new();

    public IngredientsTests(DatabaseFixture fixture)
    {
        connectionFactory = fixture.ConnectionFactory;
        var repository = new IngredientsRepository(new DapperExecutor(connectionFactory));
        service = new IngredientsService(repository);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllIngredientsAsync_ShouldReturnInsertedIngredients()
    {
        try
        {
            // Arrange
            int firstId = await InsertTestIngredientAsync("First Service Ingredient", 12345);
            int secondId = await InsertTestIngredientAsync("Second Service Ingredient", 12345);

            // Act
            var result = (await service.GetAllIngredientsAsync()).ToList();

            // Assert
            var first = Assert.Single(result.Where(i => i.Id == firstId));
            var second = Assert.Single(result.Where(i => i.Id == secondId));
            Assert.Equal("First Service Ingredient", first.Name);
            Assert.Equal("Second Service Ingredient", second.Name);
            Assert.Equal(12345, first.User_id);
            Assert.Equal(12345, second.User_id);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllIngredientsByUserAsync_ShouldReturnOnlyMatchingIngredients()
    {
        try
        {
            // Arrange
            int userId = 12345;
            int matchingId = await InsertTestIngredientAsync("User Service Ingredient", userId);
            int otherId = await InsertTestIngredientAsync("Other User Service Ingredient", 67890);

            // Act
            var result = (await service.GetAllIngredientsByUserAsync(userId)).ToList();

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
    public async Task GetAllIngredientsByUserAsync_ShouldReturnEmptyForUnknownUser()
    {
        // Arrange: generated IDs and user IDs used by these tests are positive.
        int unknownUserId = -1;

        // Act
        var result = await service.GetAllIngredientsByUserAsync(unknownUserId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetIngredientByIdAsync_ShouldReturnCorrectIngredient()
    {
        try
        {
            // Arrange
            int id = await InsertTestIngredientAsync("Service Test Ingredient", 12345);

            // Act
            var result = await service.GetIngredientByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Service Test Ingredient", result.Name);
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
    public async Task GetIngredientByIdAsync_ShouldReturnNullForMissingIngredient()
    {
        // Act
        var result = await service.GetIngredientByIdAsync(-1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateIngredientAsync_ShouldPersistCorrectIngredientData()
    {
        try
        {
            // Arrange
            var ingredient = new IngredientDTO
            {
                Id = 0,
                Name = "Created Service Ingredient",
                User_id = 12345,
                Image_id = null
            };

            // Act
            int id = await service.CreateIngredientAsync(ingredient);
            if (id > 0)
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
    public async Task UpdateIngredientAsync_ShouldPersistCorrectIngredientData()
    {
        try
        {
            // Arrange
            int id = await InsertTestIngredientAsync("Original Service Ingredient", 12345);
            int otherId = await InsertTestIngredientAsync("Unchanged Service Ingredient", 12345);
            var updated = new IngredientDTO
            {
                Id = id,
                Name = "Updated Service Ingredient",
                User_id = 67890,
                Image_id = null
            };

            // Act
            int affectedRows = await service.UpdateIngredientAsync(updated);

            // Assert
            Assert.Equal(1, affectedRows);
            var stored = await GetStoredIngredientAsync(id);
            Assert.NotNull(stored);
            Assert.Equal(id, stored.Value.Id);
            Assert.Equal("Updated Service Ingredient", stored.Value.Name);
            Assert.Equal(67890, stored.Value.UserId);
            Assert.Null(stored.Value.ImageId);

            var unchanged = await GetStoredIngredientAsync(otherId);
            Assert.NotNull(unchanged);
            Assert.Equal("Unchanged Service Ingredient", unchanged.Value.Name);
            Assert.Equal(12345, unchanged.Value.UserId);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteIngredientAsync_ShouldRemoveOnlySpecifiedIngredient()
    {
        try
        {
            // Arrange
            int deletedId = await InsertTestIngredientAsync("Service Ingredient To Delete", 12345);
            int keptId = await InsertTestIngredientAsync("Service Ingredient To Keep", 12345);

            // Act
            int affectedRows = await service.DeleteIngredientAsync(deletedId);

            // Assert
            Assert.Equal(1, affectedRows);
            Assert.Null(await GetStoredIngredientAsync(deletedId));
            var remaining = await GetStoredIngredientAsync(keptId);
            Assert.NotNull(remaining);
            Assert.Equal("Service Ingredient To Keep", remaining.Value.Name);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    // Prepare test records using SQL, independently of the service methods being tested.
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
