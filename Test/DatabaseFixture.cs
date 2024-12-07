using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Migrations;
using Infisical.Sdk;
using Microsoft.Extensions.Configuration;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Repository;

public class DatabaseFixture : IDisposable
{
    private readonly PostgreSQLConnectionFactory factory;
    private string host;
    private string port;
    private string database;
    private string username;
    private string password;

    public DatabaseFixture()
    {
        // Load settings from settings.json
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // Use the base directory of the application
            .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
            .Build();

        // Read and validate required values
        host = GetRequiredSetting(configurationRoot, "Database:Host");
        port = GetRequiredSetting(configurationRoot, "Database:Port");
        database = GetRequiredSetting(configurationRoot, "Database:Database");
        username = GetRequiredSetting(configurationRoot, "Database:Username");
        password = GetRequiredSetting(configurationRoot, "Database:Password");

        factory = new PostgreSQLConnectionFactory(host, port, database, username, password);

        ApplyMigrations();
    }

    private string GetRequiredSetting(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            throw new ApplicationException($"Missing required configuration value for key: {key}");
        }
        return value;
    }

    public PostgreSQLConnectionFactory GetFactory() => factory;

    public async Task<int> CreateTestUser()
    {
        var sql = "INSERT INTO users (first_name, last_name, email, password, role, role_approved) VALUES ('test', 'test', 'test@example.com', '1234', 'Parent', 'true') RETURNING id";
        using (var connection = factory.Create())
        {
            connection.Open();
            return await connection.QuerySingleAsync<int>(sql);
        }
    }

    public async Task<int> CreateTestImage(int userId)
    {
        var sql = $"INSERT INTO food_image (image_id, user_id, image_name, image_file_type, size) VALUES ('1', '{userId}', 'test', 'none', '0') RETURNING id";
        using (var connection = factory.Create())
        {
            connection.Open();
            return await connection.QuerySingleAsync<int>(sql);
        }
    }

    public async Task<int> CreateTestMeal(int userId, int imageId)
    {
        var sql = $"INSERT INTO meals (name, user_id, food_image_id, date) VALUES ('test', '{userId}', '{imageId}', 'test') RETURNING id";
        using (var connection = factory.Create())
        {
            connection.Open();
            return await connection.QuerySingleAsync<int>(sql);
        }
    }

    public async Task<int> CreateTestIngredient(int userId, int imageId)
    {
        var sql = $"INSERT INTO ingredients (name, user_id, food_image_id) VALUES ('test', '{userId}', '{imageId}') RETURNING id";
        using (var connection = factory.Create())
        {
            connection.Open();
            return await connection.QuerySingleAsync<int>(sql);
        }
    }

    public async void ResetDatabase()
    {
        var sql = "TRUNCATE TABLE users CASCADE";
        using (var connection = factory.Create())
        {
            connection.Open();
            await connection.ExecuteAsync(sql);
        }
    }

    private void ApplyMigrations()
    {
        using var scope = CreateMigrationScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        if (runner.HasMigrationsToApplyUp())
        {
            runner.ListMigrations();
            runner.MigrateUp();
        }
    }

    private IServiceScope CreateMigrationScope()
    {
        var serviceCollection = new ServiceCollection();

        // Add FluentMigrator services for PostgreSQL
        serviceCollection.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(
                    $"Host={host};" +
                    $"Port={port};" +
                    $"Database={database};" +
                    $"Username={username};" +
                    $"Password={password}")
                .ScanIn(typeof(InitTables).Assembly).For.Migrations());

        return serviceCollection.BuildServiceProvider().CreateScope();
    }

    public void Dispose()
    {
        ResetDatabase();
    }
}