using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Npgsql;

namespace Test.Integration.Repositories;

[Collection("IntegrationDatabase")]
public class MealTests
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly MealsRepository repository;

    public MealTests(DatabaseFixture fixture)
    {
        connectionFactory = fixture.ConnectionFactory;

        repository = new MealsRepository(
            new DapperExecutor(connectionFactory));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InsertAsync_ShouldPersistCorrectMealData()
    {
        // Arrange
        int userId = 12345;

        var meal = new MealCreateDTO
        {
            Name = "Test Meal",
            Date = "2026-11-02",
            Template = false,
            Image_id = null
        };

        int mealId = 0;

        try
        {
            // Act
            mealId = await repository.InsertAsync(meal, userId);

            // Assert
            Assert.True(mealId > 0);

            var stored = await GetStoredMealAsync(mealId);

            Assert.NotNull(stored);
            Assert.Equal(mealId, stored.Value.Id);
            Assert.Equal(userId, stored.Value.UserId);
            Assert.Equal(meal.Name, stored.Value.Name);
            Assert.Equal(meal.Date, stored.Value.Date);
            Assert.Equal(meal.Template, stored.Value.Template);
            Assert.True(stored.Value.ImageIdIsNull);
        }
        finally
        {
            if (mealId > 0)
                await DeleteTestMealAsync(mealId);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsync_ShouldPersistCorrectMealData()
    {
        // Arrange
        int originalUserId = 12345;
        int updatedUserId = 67890;

        int mealId = 0;

        try
        {
            mealId = await InsertTestMealAsync(
                "Original Meal", originalUserId, "2026-11-02", false);

            var updatedMeal = new MealDTO
            {
                Id = mealId,
                Name = "Updated Meal",
                User_id = updatedUserId,
                Date = "2026-11-03",
                Template = true,
                Image_id = null
            };

            // Act
            int affectedRows = await repository.UpdateAsync(updatedMeal);

            // Assert
            Assert.Equal(1, affectedRows);

            var stored = await GetStoredMealAsync(mealId);

            Assert.NotNull(stored);
            Assert.Equal(mealId, stored.Value.Id);
            Assert.Equal(updatedUserId, stored.Value.UserId);
            Assert.Equal(updatedMeal.Name, stored.Value.Name);
            Assert.Equal(updatedMeal.Date, stored.Value.Date);
            Assert.Equal(updatedMeal.Template, stored.Value.Template);
            Assert.True(stored.Value.ImageIdIsNull);
        }
        finally
        {
            // Cleanup
            if (mealId > 0)
                await DeleteTestMealAsync(mealId);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_ShouldReturnInsertedMeal()
    {
        // Arrange
        int userId = 12345;

        int mealId = 0;

        try
        {
            mealId = await InsertTestMealAsync(
                "Test Meal", userId, "2026-11-02", false);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);

            var meal = Assert.Single(
                result.Where(m => m.Id == mealId));

            Assert.Equal(mealId, meal.Id);
            Assert.Equal(userId, meal.User_id);
            Assert.Equal("Test Meal", meal.Name);
            Assert.Equal("2026-11-02", meal.Date);
            Assert.False(meal.Template);
            Assert.Null(meal.Image_id);
        }
        finally
        {
            // Cleanup
            if (mealId > 0)
                await DeleteTestMealAsync(mealId);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_ShouldReturnCorrectMeal()
    {
        // Arrange
        int userId = 12345;

        int mealId = 0;

        try
        {
            mealId = await InsertTestMealAsync(
                "Test Meal", userId, "2026-11-02", false);

            // Act
            var result = await repository.GetByIdAsync(mealId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mealId, result.Id);
            Assert.Equal(userId, result.User_id);
            Assert.Equal("Test Meal", result.Name);
            Assert.Equal("2026-11-02", result.Date);
            Assert.False(result.Template);
            Assert.Null(result.Image_id);
        }
        finally
        {
            // Cleanup
            if (mealId > 0)
                await DeleteTestMealAsync(mealId);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllByUserAndDateAsync_ShouldReturnOnlyMatchingMeals()
    {
        // Arrange
        int userId = 12345;
        string date = "2026-11-02";

        var mealIds = new List<int>();

        try
        {
            mealIds.Add(await InsertTestMealAsync(
                "Matching Meal", userId, date, false));

            mealIds.Add(await InsertTestMealAsync(
                "Wrong User", 67890, date, false));

            mealIds.Add(await InsertTestMealAsync(
                "Wrong Date", userId, "2026-11-03", false));

            // Act
            var result = (await repository
                .GetAllByUserAndDateAsync(userId, date)).ToList();

            // Assert
            Assert.Contains(result, m => m.Id == mealIds[0]);

            Assert.DoesNotContain(result, m => m.Id == mealIds[1]);
            Assert.DoesNotContain(result, m => m.Id == mealIds[2]);

            Assert.All(result, m =>
            {
                Assert.Equal(userId, m.User_id);
                Assert.Equal(date, m.Date);
            });
        }
        finally
        {
            // Cleanup
            foreach (int id in mealIds)
                await DeleteTestMealAsync(id);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllTemplatesByUserAsync_ShouldReturnOnlyUserTemplates()
    {
        // Arrange
        int userId = 12345;

        var mealIds = new List<int>();

        try
        {
            mealIds.Add(await InsertTestMealAsync(
                "User Template", userId, "2026-11-02", true));

            mealIds.Add(await InsertTestMealAsync(
                "Not Template", userId, "2026-11-02", false));

            mealIds.Add(await InsertTestMealAsync(
                "Other User Template", 67890, "2026-11-02", true));

            // Act
            var result = (await repository
                .GetAllTemplatesByUserAsync(userId)).ToList();

            // Assert
            Assert.Contains(result, m => m.Id == mealIds[0]);

            Assert.DoesNotContain(result, m => m.Id == mealIds[1]);
            Assert.DoesNotContain(result, m => m.Id == mealIds[2]);

            Assert.All(result, m =>
            {
                Assert.Equal(userId, m.User_id);
                Assert.True(m.Template);
            });
        }
        finally
        {
            // Cleanup
            foreach (int id in mealIds)
                await DeleteTestMealAsync(id);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateTemplateStatusAsync_ShouldUpdateCorrectMeal()
    {
        // Arrange
        var mealIds = new List<int>();

        try
        {
            mealIds.Add(await InsertTestMealAsync(
                "Test Meal", 12345, "2026-11-02", false));

            mealIds.Add(await InsertTestMealAsync(
                "Other Meal", 67890, "2026-11-02", false));

            int mealId = mealIds[0];

            // Act
            int affectedRows = await repository
                .UpdateTemplateStatusAsync(mealId, true);

            // Assert
            Assert.Equal(1, affectedRows);

            var stored = await GetStoredMealAsync(mealId);
            var other = await GetStoredMealAsync(mealIds[1]);

            Assert.NotNull(stored);
            Assert.NotNull(other);

            Assert.True(stored.Value.Template);
            Assert.False(other.Value.Template);

            Assert.Equal(12345, stored.Value.UserId);
        }
        finally
        {
            // Cleanup
            foreach (int id in mealIds)
                await DeleteTestMealAsync(id);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteAsync_ShouldRemoveCorrectMeal()
    {
        // Arrange
        var mealIds = new List<int>();

        try
        {
            mealIds.Add(await InsertTestMealAsync(
                "Meal To Delete", 12345, "2026-11-02", false));

            mealIds.Add(await InsertTestMealAsync(
                "Meal To Keep", 67890, "2026-11-02", false));

            // Act
            int affectedRows = await repository.DeleteAsync(mealIds[0]);

            // Assert
            Assert.Equal(1, affectedRows);

            var deletedMeal = await GetStoredMealAsync(mealIds[0]);
            var remainingMeal = await GetStoredMealAsync(mealIds[1]);

            Assert.Null(deletedMeal);
            Assert.NotNull(remainingMeal);
            Assert.Equal("Meal To Keep", remainingMeal.Value.Name);
        }
        finally
        {
            // Cleanup
            foreach (int id in mealIds)
                await DeleteTestMealAsync(id);
        }
    }

    // Inserts test data directly into PostgreSQL
    private async Task<int> InsertTestMealAsync(
        string name,
        int userId,
        string date,
        bool template)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "INSERT INTO meals (name, user_id, image_id, date, template) " +
            "VALUES (@name, @userId, NULL, @date, @template) RETURNING id",
            connection);

        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("userId", userId);
        command.Parameters.AddWithValue("date", date);
        command.Parameters.AddWithValue("template", template);

        return (int)(await command.ExecuteScalarAsync())!;
    }

    // Reads persisted data directly from PostgreSQL
    private async Task<(
        int Id,
        int UserId,
        string Name,
        string Date,
        bool Template,
        bool ImageIdIsNull)?> GetStoredMealAsync(int id)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT id, user_id, name, date, template, image_id " +
            "FROM meals WHERE id = @id",
            connection);

        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return (
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetBoolean(4),
            reader.IsDBNull(5)
        );
    }

    // Removes test data from PostgreSQL
    private async Task DeleteTestMealAsync(int id)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "DELETE FROM meals WHERE id = @id",
            connection);

        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}