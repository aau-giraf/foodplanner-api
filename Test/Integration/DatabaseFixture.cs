using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Migrations;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;

using Microsoft.Extensions.DependencyInjection;

using Npgsql;

namespace Test.Integration
{
    public class DatabaseFixture
    {
        public PostgreSQLConnectionFactory ConnectionFactory { get; }

        public DatabaseFixture()
        {
            // Get test database configuration
            var host = "localhost";
            var port = "7656";
            var database = "giraf_test";
            var username = "postgres";
            var password = "postgres";
            // Ensure that migrations only run on the test database
            if (!database.Equals("giraf_test", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Integration tests require the giraf_test database.");
            }

            // Create database connection factory
            ConnectionFactory = new PostgreSQLConnectionFactory(
                host, port, database, username, password);

            var connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = int.Parse(port),
                Database = database,
                Username = username,
                Password = password
            }.ConnectionString;

            // Configure FluentMigrator
            using var serviceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(InitTables).Assembly).For.Migrations())
                .Configure<RunnerOptions>(options =>
                {
                    options.Tags = new[] { "FoodPlanner" };
                    options.IncludeUntaggedMigrations = false;
                })
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider();

            // Run migrations
            using var scope = serviceProvider.CreateScope();

            var runner = scope.ServiceProvider
                .GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }

        private static string GetRequiredSetting(string name)
        {
            return Environment.GetEnvironmentVariable(name)
                ?? throw new InvalidOperationException(
                    $"Missing environment variable: {name}");
        }
    }
}