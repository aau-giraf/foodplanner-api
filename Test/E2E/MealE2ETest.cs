using System.Net;
using System.Net.Http.Json;
using FoodplannerDataAccessSql;
using FoodplannerModels.Lunchbox;
using Npgsql;

namespace Test.E2E;

[Collection("E2EDatabase")]
public class MealE2ETest : IDisposable
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly HttpClient client;
    private const int UserId = 12345;

    public MealE2ETest(
        DatabaseFixture fixture,
        E2EWebApplicationFactory factory)
    {
        connectionFactory = fixture.ConnectionFactory;
        client = TestAuthentication.CreateAuthenticatedClient(factory, UserId);
    }

    public void Dispose()
    {
        client.Dispose();
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task Create_CreateMeal()
    {
        // Arrange
        var meal = new MealCreateDTO
        {
            Name = "E2E_Create_Meal",
            Date = DateOnly.Parse("2026/11/02"),
            Template = false,
            Image_id = null
        };

        try
        {
            // Act
            using var response = await client.PostAsJsonAsync("/api/Meals/Create", meal);

            // Assert HTTP response
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdMeal = await response.Content.ReadFromJsonAsync<MealWithIngredientsDTO>();

            Assert.NotNull(createdMeal);
            Assert.True(createdMeal.Id > 0);
            Assert.Equal(meal.Name, createdMeal.Name);
            Assert.Equal(UserId, createdMeal.User_id);
            Assert.Equal(meal.Date, createdMeal.Date);
            Assert.Equal(meal.Template, createdMeal.Template);
            Assert.Null(createdMeal.Image_id);
            Assert.Empty(createdMeal.Ingredients);

            // Assert persisted database data
            var stored = await GetStoredMealAsync(createdMeal.Id);

            Assert.NotNull(stored);
            Assert.Equal(createdMeal.Id, stored.Value.Id);
            Assert.Equal(UserId, stored.Value.UserId);
            Assert.Equal(meal.Name, stored.Value.Name);
            Assert.Equal(meal.Date, stored.Value.Date);
            Assert.Equal(meal.Template, stored.Value.Template);
            Assert.True(stored.Value.ImageIdIsNull);
        }
        finally
        {
            // Cleanup
            await DeleteTestMealByNameAsync(meal.Name);
        }
    }



    //Helper functions
    // Reads persisted data directly from PostgreSQL
    private async Task<(int Id, int UserId, string Name, DateOnly Date, bool Template, bool ImageIdIsNull)?> GetStoredMealAsync(int id)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand("SELECT id, user_id, name, date, template, image_id FROM meals WHERE id = @id", connection);

        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return (
            reader.GetInt32(0),reader.GetInt32(1),reader.GetString(2),
            reader.GetFieldValue<DateOnly>(3),reader.GetBoolean(4),reader.IsDBNull(5)
            );
    }

    // Removes test data directly from PostgreSQL
    private async Task DeleteTestMealByNameAsync(string name)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand("DELETE FROM meals WHERE name = @name",connection);

        command.Parameters.AddWithValue("name", name);

        await command.ExecuteNonQueryAsync();
    }

}

