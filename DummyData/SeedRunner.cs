using DummyData.CoreMock;
using DummyData.Foodplanner;
using FoodplannerServices.Secret;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DummyData;

public static class SeedRunner
{
    public static async Task RunAsync(
        string target,
        IConfiguration configuration)
    {
        // Initialize SecretsLoader
        var secretsLoader = new SecretsLoader(
            configuration,
            "Development"
        );

        if (target is "all" or "core")
        {
            var connectionString = BuildConnectionString(
                secretsLoader,
                "CORE_DB"
            );

            await SeedDatabaseAsync(
                connectionString,
                CoreMockSeeder.SeedAsync
            );
        }

        if (target is "all" or "foodplanner")
        {
            var connectionString = BuildConnectionString(
                secretsLoader,
                "DB"
            );

            await SeedDatabaseAsync(
                connectionString,
                FoodplannerSeeder.SeedAsync
            );
        }

        Console.WriteLine("Dummy data generation completed.");
    }

    private static string BuildConnectionString(
        SecretsLoader secretsLoader,
        string prefix)
    {
        var connectionBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = secretsLoader.GetSecret($"{prefix}_HOST"),
            Port = int.Parse(secretsLoader.GetSecret($"{prefix}_PORT")),
            Database = secretsLoader.GetSecret($"{prefix}_NAME"),
            Username = secretsLoader.GetSecret($"{prefix}_USER"),
            Password = secretsLoader.GetSecret($"{prefix}_PASS")
        };

        return connectionBuilder.ConnectionString;
    }

    private static async Task SeedDatabaseAsync(
        string connectionString,
        Func<NpgsqlConnection, NpgsqlTransaction, Task> seeder)
    {
        await using var connection =
            new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        try
        {
            await seeder(connection, transaction);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}