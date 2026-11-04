using DummyData.CoreMock;
using DummyData.Foodplanner;
using FoodplannerServices.Secret;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DummyData;

public static class SeedRunner
{
    public static async Task RunAsync(string target,IConfiguration configuration)
    {
        // Initialize SecretsLoader
        var secretsLoader = new SecretsLoader(configuration,"Development");

        //check if command targets all or core, if so build connection to core db
        if (ShouldSeed(target, "core"))
        {
            var connectionString = BuildConnectionString(key => secretsLoader.GetSecret(key),"CORE_DB");
            //await core seeders
            await SeedDatabaseAsync(connectionString,CoreMockSeeder.SeedAsync);
        }
        //check if command targets all or foodplanner, if so build connection to foodplanner db
        if (ShouldSeed(target, "foodplanner"))
        {
            var connectionString = BuildConnectionString(key => secretsLoader.GetSecret(key),"DB");
            //await foodplanner seeders
            await SeedDatabaseAsync(connectionString,FoodplannerSeeder.SeedAsync);
        }

        Console.WriteLine("Dummy data generation completed.");
    }

    // Build a PostgreSQL connection string using the provided secret values
    // The prefix determines whether CoreMock or Foodplanner settings are used
    public static string BuildConnectionString(Func<string, string> getSecret,string prefix)
    {
        var connectionBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = getSecret($"{prefix}_HOST"),
            Port = int.Parse(getSecret($"{prefix}_PORT")),
            Database = getSecret($"{prefix}_NAME"),
            Username = getSecret($"{prefix}_USER"),
            Password = getSecret($"{prefix}_PASS")
        };

        return connectionBuilder.ConnectionString;
    }

    //database insertion function
    private static async Task SeedDatabaseAsync(string connectionString,Func<NpgsqlConnection, NpgsqlTransaction, Task> seeder)
    {
        //await database connection
        await using var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

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

    // Check if the selected target should seed the specified database
    // Returns true if the target is "all" or matches the database
    public static bool ShouldSeed(string target, string database)
    {
        return target == "all" || target == database;
    }
}

