using Npgsql;

namespace DummyData.CoreMock;

public static class ChildrenSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding children...");

        // Dummy children
        var children = new[]
        {
            new
            {
                child_id = 3,
                first_name = "Test",
                last_name = "Child",
                class_id = 1,
            },
            new
            {
                child_id = 4,
                first_name = "Sebastian",
                last_name = "Jensen",
                class_id = 1,
            },
            new
            {
                child_id = 5,
                first_name = "Sara",
                last_name = "Jensen",
                class_id = 2,
            },
            new
            {
                child_id = 6,
                first_name = "Frederik",
                last_name = "Jensen",
                class_id = 2,
            },
        };

        //insert each child one at a time
        foreach (var child in children)
        {
            const string sql = """
                INSERT INTO children (
                    child_id,
                    first_name,
                    last_name,
                    class_id
                )
                SELECT
                    @child_id,
                    @first_name,
                    @last_name,
                    @class_id
                WHERE NOT EXISTS (
                    SELECT 1 FROM children WHERE first_name = @first_name
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("child_id", child.child_id);
            command.Parameters.AddWithValue("first_name", child.first_name);
            command.Parameters.AddWithValue("last_name", child.last_name);
            command.Parameters.AddWithValue("class_id", child.class_id);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("children seeded successfully.");
    }
}