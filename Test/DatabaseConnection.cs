using FoodplannerDataAccessSql;
using Dapper;

namespace testing;

static class DatabaseConnection
{
    //Until fixed, you need to fill it in your self.
    private static string host = "";
    private static string port = "";
    private static string database = "";
    private static string username = "";
    private static string password = "";

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
}