using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;

using Npgsql;
using Xunit;

using Test.Integration;

[Collection("IntegrationDatabase")]
public class IngredientsRepositorytests
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly IngredientsRepository repository;

    public IngredientsRepositorytests(DatabaseFixture fixture)
    {
        connectionFactory = fixture.ConnectionFactory;

        repository = new IngredientsRepository(
            new DapperExecutor(connectionFactory));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_ReturnsInsertedIngredientFromDatabase()
    {
        // Arrange
        var ingredientName = $"TestIngredient";
        var userId = 12345;

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        await using var insertCommand = new NpgsqlCommand(
            "INSERT INTO ingredients (name, user_id, image_id) " +
            "VALUES (@name, @userId, NULL) RETURNING id",
            connection);

        insertCommand.Parameters.AddWithValue("name", ingredientName);
        insertCommand.Parameters.AddWithValue("userId", userId);

        var ingredientId = (int)(
            await insertCommand.ExecuteScalarAsync())!;

        try
        {
            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);

            var ingredient = Assert.Single(
                result.Where(i => i.Id == ingredientId));

            Assert.Equal(ingredientId, ingredient.Id);
            Assert.Equal(ingredientName, ingredient.Name);
            Assert.Equal(userId, ingredient.User_id);
            Assert.Null(ingredient.Image_id);
        }
        finally
        {
            // Cleanup
            await using var deleteCommand = new NpgsqlCommand(
                "DELETE FROM ingredients WHERE id = @id",
                connection);

            deleteCommand.Parameters.AddWithValue("id", ingredientId);

            await deleteCommand.ExecuteNonQueryAsync();
        }
    }
}