using DummyData.Foodplanner;
using Npgsql;

namespace Test.Integration.Seeder;

[Collection("IntegrationDatabase")]
public class FoodplannerSubseederIntegrationTests
{
    private readonly DatabaseFixture fixture;

    public FoodplannerSubseederIntegrationTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task FoodplannerSeeder_ShouldSeedAllSixTablesAndTheirRelationships()
    {
        // Arrange
        await using var connection = fixture.ConnectionFactory.Create();
        Assert.Equal("giraf_test", new NpgsqlConnectionStringBuilder(connection.ConnectionString).Database);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await ResetTablesAsync(connection, transaction);

        // Act
        await FoodplannerSeeder.SeedAsync(connection, transaction);

        // Assert
        Assert.Equal(4, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM meals"));
        Assert.Equal(16, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM ingredients"));
        Assert.Equal(11, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM subingredient"));
        Assert.Equal(5, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM packed"));
        Assert.Equal(11, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM categories"));
        Assert.Equal(16, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM tags"));

        Assert.Equal(1, await CountAsync(connection, transaction,
            "SELECT COUNT(*) FROM meals WHERE name = 'Test''s Madpakke'"));
        Assert.Equal(1, await CountAsync(connection, transaction,
            "SELECT COUNT(*) FROM ingredients WHERE name = 'Lasagne'"));
        Assert.Equal(1, await CountAsync(connection, transaction,
            "SELECT COUNT(*) FROM categories WHERE name = 'Frugt og Grønt' AND main = true"));

        Assert.Equal(1, await CountAsync(connection, transaction, """
            SELECT COUNT(*) FROM subingredient s
            JOIN ingredients parent ON parent.id = s.parent_id
            JOIN ingredients child ON child.id = s.child_id
            WHERE parent.name = 'Lasagne' AND child.name = 'Pasta Plader'
            """));
        Assert.Equal(1, await CountAsync(connection, transaction, """
            SELECT COUNT(*) FROM packed p
            JOIN meals m ON m.id = p.meal_id
            JOIN ingredients i ON i.id = p.ingredient_id
            WHERE m.name = 'Test''s Madpakke' AND i.name = 'Lasagne'
            """));
        Assert.Equal(1, await CountAsync(connection, transaction, """
            SELECT COUNT(*) FROM tags t
            JOIN ingredients i ON i.id = t.ingredient_id
            JOIN categories c ON c.id = t.category_id
            WHERE i.name = 'Æble' AND c.name = 'Frugt og Grønt'
            """));

        await transaction.RollbackAsync();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task FoodplannerSeeder_ShouldNotInsertDuplicatesWhenCalledTwice()
    {
        // Arrange
        await using var connection = fixture.ConnectionFactory.Create();
        Assert.Equal("giraf_test", new NpgsqlConnectionStringBuilder(connection.ConnectionString).Database);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await ResetTablesAsync(connection, transaction);

        // Act
        await FoodplannerSeeder.SeedAsync(connection, transaction);
        await FoodplannerSeeder.SeedAsync(connection, transaction);

        // Assert
        Assert.Equal(4, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM meals"));
        Assert.Equal(16, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM ingredients"));
        Assert.Equal(11, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM subingredient"));
        Assert.Equal(5, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM packed"));
        Assert.Equal(11, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM categories"));
        Assert.Equal(16, await CountAsync(connection, transaction, "SELECT COUNT(*) FROM tags"));

        await transaction.RollbackAsync();
    }

    private static async Task ResetTablesAsync(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        await using var command = new NpgsqlCommand("""
            TRUNCATE TABLE tags, packed, subingredient, categories, meals, ingredients
            RESTART IDENTITY CASCADE;
            """, connection, transaction);
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<long> CountAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        return (long)(await command.ExecuteScalarAsync())!;
    }
}
