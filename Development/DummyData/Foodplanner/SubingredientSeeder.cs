using Npgsql;

namespace DummyData.Foodplanner;

public static class SubingredientSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding subingredients...");

        // Dummy subingredients
        var subingredients = new[]
        {
            new
            {
                parent_id = 1,
                child_id = 2,
                order_no = 1,
            },
            new
            {
                parent_id = 1,
                child_id = 3,
                order_no = 2,
            },
            new
            {
                parent_id = 3,
                child_id = 4,
                order_no = 1,
            },
            new
            {
                parent_id = 1,
                child_id = 5,
                order_no = 3,
            },
            new
            {
                parent_id = 1,
                child_id = 6,
                order_no = 4,
            },
            new
            {
                parent_id = 7,
                child_id = 8,
                order_no = 1,
            },
            new
            {
                parent_id = 9,
                child_id = 10,
                order_no = 1,
            },
            new
            {
                parent_id = 9,
                child_id = 11,
                order_no = 2,
            },
            new
            {
                parent_id = 12,
                child_id = 13,
                order_no = 1,
            },
            new
            {
                parent_id = 12,
                child_id = 14,
                order_no = 2,
            },
            new
            {
                parent_id = 12,
                child_id = 15,
                order_no = 3,
            },
        };

        //insert each subingredient one at a time
        foreach (var subingredient in subingredients)
        {
            const string sql = """
                INSERT INTO subingredient (
                    parent_id,
                    child_id,
                    order_no
                )
                SELECT
                    @parent_id,
                    @child_id,
                    @order_no
                WHERE NOT EXISTS (
                    SELECT 1 FROM subingredient WHERE parent_id = @parent_id AND child_id = @child_id
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("parent_id", subingredient.parent_id);
            command.Parameters.AddWithValue("child_id", subingredient.child_id);
            command.Parameters.AddWithValue("order_no", subingredient.order_no);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("subingredients seeded successfully.");
    }
}