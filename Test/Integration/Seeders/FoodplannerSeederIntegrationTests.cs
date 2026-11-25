using DummyData;
using DummyData.Foodplanner;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace Test.Integration.Seeder;

[Collection("IntegrationDatabase")]
public class FoodplannerSeederIntegrationTests
{
    private readonly DatabaseFixture fixture;

    public FoodplannerSeederIntegrationTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SeedRunner_ShouldExecuteFoodplannerSeederAgainstTestDatabase()
    {
        // Arrange
        await using var connection = fixture.ConnectionFactory.Create();
        var settings = new NpgsqlConnectionStringBuilder(connection.ConnectionString);
        Assert.Equal("giraf_test", settings.Database);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        // Reset test tables and IDs. The transaction is rolled back after the test.
        await using (var reset = new NpgsqlCommand("""
            TRUNCATE TABLE tags, packed, subingredient, categories, meals, ingredients
            RESTART IDENTITY CASCADE;
            """, connection, transaction))
        {
            await reset.ExecuteNonQueryAsync();
        }

        string GetTestSecret(string key) => (key switch
        {
            "DB_HOST" => settings.Host,
            "DB_PORT" => settings.Port.ToString(),
            "DB_NAME" => settings.Database,
            "DB_USER" => settings.Username,
            "DB_PASS" => settings.Password,
            _ => throw new ArgumentException($"Unexpected secret key: {key}")
        }) ?? throw new InvalidOperationException($"Missing test database setting: {key}");

        int calls = 0;

        // Act
        await SeedRunner.RunAsync("foodplanner", new ConfigurationBuilder().Build(),
            GetTestSecret,
            async (connectionString, seeder) =>
            {
                calls++;
                Assert.Equal(typeof(FoodplannerSeeder), seeder.Method.DeclaringType);
                Assert.Equal(settings.Database,
                    new NpgsqlConnectionStringBuilder(connectionString).Database);

                await seeder(connection, transaction);
            });

        // Assert
        Assert.Equal(1, calls);
        await using var command = new NpgsqlCommand(
            "SELECT COUNT(*) FROM meals", connection, transaction);
        var mealCount = (long)(await command.ExecuteScalarAsync())!;
        Assert.True(mealCount > 0);

    }
}
