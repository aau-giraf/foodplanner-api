using Npgsql;

namespace DummyData.CoreMock;

public static class ChildrenRelationSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding children relations...");

        // Dummy children relations
        var children_relations = new[]
        {
            //Parents
            new
            {
                user_id = 1,
                child_id = 3,
            },
            new
            {
                user_id = 2,
                child_id = 4,
            },
            new
            {
                user_id = 2,
                child_id = 5,
            },
            new
            {
                user_id = 2,
                child_id = 6,
            },
            //Teachers
            new
            {
                user_id = 7,
                child_id = 3,
            },
            new
            {
                user_id = 7,
                child_id = 4,
            },
            new
            {
                user_id = 8,
                child_id = 5,
            },
            new
            {
                user_id = 8,
                child_id = 6,
            },
        };

        //insert each child relation one at a time
        foreach (var child_relation in children_relations)
        {
            const string sql = """
                INSERT INTO child_relation (
                    user_id,
                    child_id
                )
                SELECT
                    @user_id,
                    @child_id
                WHERE NOT EXISTS (
                    SELECT 1 FROM child_relation WHERE user_id = @user_id AND child_id = @child_id
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("user_id", child_relation.user_id);
            command.Parameters.AddWithValue("child_id", child_relation.child_id);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("children relations seeded successfully.");
    }
}