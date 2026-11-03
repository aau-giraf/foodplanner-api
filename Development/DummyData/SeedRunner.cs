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

        //check if command targets all or core, if so build connection to core db
        if (target is "all" or "core")
        {
            var connectionString = BuildConnectionString(
                secretsLoader,
                "CORE_DB"
            );
            //await core seeders
            await SeedDatabaseAsync(
                connectionString,
                CoreMockSeeder.SeedAsync
            );
        }
        //check if command targets all or foodplanner, if so build connection to foodplanner db
        if (target is "all" or "foodplanner")
        {
            var connectionString = BuildConnectionString(
                secretsLoader,
                "DB"
            );
            //await foodplanner seeders
            await SeedDatabaseAsync(
                connectionString,
                FoodplannerSeeder.SeedAsync
            );
        }

        Console.WriteLine("Dummy data generation completed.");
    }
    //database connection builder
    private static string BuildConnectionString(
        SecretsLoader secretsLoader,
        string prefix)
    {
        //getting infiscial acces, based on CORE_DB or DB
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

    //database insertion function
    private static async Task SeedDatabaseAsync(
        string connectionString,
        Func<NpgsqlConnection, NpgsqlTransaction, Task> seeder)
    {
        //await database connection
        await using var connection =
            new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        //try to call seeders, if seeders succed commit changes to DB, if seeders fail rollback all changes made
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