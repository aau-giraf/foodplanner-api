using Npgsql;

namespace DummyData.CoreMock;

public static class UserSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding users...");

        // Dummy users
        var users = new[]
        {
            new
            {
                first_name = "Test",
                last_name = "Parent",
                email = "test@example.com",
                password = BCrypt.Net.BCrypt.HashPassword("1234"),
                role = "Parent",
                pincode = BCrypt.Net.BCrypt.HashPassword("1234"),
                role_approved = true,
            }
        };

        foreach (var user in users)
        {
            const string sql = """
                INSERT INTO users (
                    first_name,
                    last_name,
                    email,
                    password,
                    role,
                    pincode,
                    role_approved
                )
                SELECT
                    @first_name,
                    @last_name,
                    @email,
                    @password,
                    @role,
                    @pincode,
                    @role_approved
                WHERE NOT EXISTS (
                    SELECT 1 FROM users WHERE email = @email
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("first_name", user.first_name);
            command.Parameters.AddWithValue("last_name", user.last_name);
            command.Parameters.AddWithValue("email", user.email);
            command.Parameters.AddWithValue("password", user.password);
            command.Parameters.AddWithValue("role", user.role);
            command.Parameters.AddWithValue("pincode", user.pincode);
            command.Parameters.AddWithValue("role_approved", user.role_approved);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("Users seeded successfully.");
    }
}