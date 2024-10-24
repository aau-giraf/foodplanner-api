using FoodplannerDataAccessSql;
using Dapper;

namespace testing;

static class DatabaseConnection
{
    private static string host = "10.92.0.69";
    private static string port = "5432";
    private static string database = "giraf_foodplanner_db_sw5_10";
    private static string username = "postgres";
    private static string password = "yjF45K#6rmRd@2RJbbLA";

    public static PostgreSQLConnectionFactory GetConnection()
    {
        return new PostgreSQLConnectionFactory(host, port, database, username, password);
    }

    public static async Task EmptyDatabase(string tableName)
    {
        // Validate the table name to prevent SQL injection
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name must not be null or empty.");

        // Ensure tableName is a valid SQL identifier
        var validTableNames = new HashSet<string> { "packed_ingredients", "meals", "ingredients" };
        if (!validTableNames.Contains(tableName))
            throw new ArgumentException($"Invalid table name: {tableName}");

        PostgreSQLConnectionFactory connectionFactory = new PostgreSQLConnectionFactory(host, port, database, username, password);
        
        // Truncate is better if we need to reset auto-increment fields
        var sql = $"TRUNCATE TABLE {tableName} RESTART IDENTITY CASCADE";
        
        using var connection = connectionFactory.Create();
        await connection.OpenAsync();  // Make sure opening the connection asynchronously
        await connection.ExecuteAsync(sql);
    }
}