using Npgsql;

namespace DummyData.CoreMock;

public static class ClassroomSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding classrooms...");

        // Dummy classrooms
        var classrooms = new[]
        {
            new
            {
                class_name = "PE",
            },
            new
            {
                class_name = "Math",
            },
        };

        //insert each classroom one at a time
        foreach (var classroom in classrooms)
        {
            const string sql = """
                INSERT INTO classroom (
                    class_name
                )
                SELECT
                    @class_name
                WHERE NOT EXISTS (
                    SELECT 1 FROM classroom WHERE class_name = @class_name
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("class_name", classroom.class_name);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("classrooms seeded successfully.");
    }
}