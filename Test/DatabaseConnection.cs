using FoodplannerDataAccessSql;
using FoodplannerApi;
using Dapper;
using Microsoft.Extensions.Configuration;
using Infisical.Sdk;
using System.Text.Json;

namespace testing;

static class DatabaseConnection
{
    private record Configuration(string EnvironmentSlug, string WorkspaceId, InfisicalClient Client);
    private static Configuration _configuration = null!;
    private static readonly PostgreSQLConnectionFactory connectionFactory = GenerateConnection();

    public static PostgreSQLConnectionFactory GetConnection()
    {
        return connectionFactory;
    }

    public static async Task EmptyDatabase(string tableName)
    {
        // Validate the table name to prevent SQL injection
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name must not be null or empty.");

        // Ensure tableName is a valid SQL identifier
        var validTableNames = new HashSet<string> { "packed_ingredients", "meals", "ingredients", "users", "food_image" };
        if (!validTableNames.Contains(tableName))
            throw new ArgumentException($"Invalid table name: {tableName}");
        
        // Truncate is better if we need to reset auto-increment fields
        var sql = $"TRUNCATE TABLE {tableName} RESTART IDENTITY CASCADE";
        
        using var connection = GetConnection().Create();
        await connection.OpenAsync();  // Make sure opening the connection asynchronously
        await connection.ExecuteAsync(sql);
    }

    public static async Task SetupTempUserAndImage()
    {
        await EmptyDatabase("food_image");
        await EmptyDatabase("users");
        using var connection = GetConnection().Create();
        await connection.OpenAsync();

        var sql = "INSERT INTO users (first_name, last_name, email, password, role, role_approved)\n";
        sql += $"VALUES ('Temp', 'Temp', 'empty', '1234', 'Test', true)";
        await connection.ExecuteAsync(sql);

        sql = "INSERT INTO food_image (user_id, image_name, image_file_type, size)\n";
        sql += $"VALUES (1, 'empty', 'none', 0)";
        await connection.ExecuteAsync(sql);
    }

    private static void Configure()
    {
        string text = File.ReadAllText(@"../../../testsettings.json");
        var content = JsonSerializer.Deserialize<DatabaseSettings>(text);
        var settings = new ClientSettings
        {
            Auth = new AuthenticationOptions
            {
                UniversalAuth = new UniversalAuthMethod
                {
                    ClientId = content.ClientId,
                    ClientSecret = content.ClientSecret,
                }
            }
        };
        
        _configuration = new Configuration("dev", content.Workspace, new InfisicalClient(settings));
    }

    private static string GetSecret(string secretName, string path = "/")
    {
        if (_configuration == null)
        {
            throw new ApplicationException($"Unable to load secret: {secretName}. SecretsLoader must be configured before usage");
        }
        
        var getSecretOptions = new GetSecretOptions
        {
            Path = path,
            SecretName = secretName,
            ProjectId = _configuration.WorkspaceId,
            Environment = _configuration.EnvironmentSlug,
        };
        
        return _configuration.Client.GetSecret(getSecretOptions).SecretValue;
    }
    
    private static PostgreSQLConnectionFactory GenerateConnection()
    {
        Configure();
        var host = GetSecret("DB_HOST", "/SW-5-10/");
        var port = GetSecret("DB_PORT", "/SW-5-10/");
        var database = GetSecret("DB_NAME", "/SW-5-10/");
        var username = GetSecret("DB_USER", "/SW-5-10/");
        var password = GetSecret("DB_PASS", "/SW-5-10/");
        return new PostgreSQLConnectionFactory(host, port, database, username, password);
    }

    private class DatabaseSettings
    {
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string Workspace { get; set; }
    }
}