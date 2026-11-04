using Npgsql;

namespace DummyData.Foodplanner;

public static class PackedSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding packed...");

        // Dummy packeds
        var packeds = new[]
        {
            new
            {
                meal_id = 1,
                ingredient_id = 1,
                order_no = 1,
            },
            new
            {
                meal_id = 1,
                ingredient_id = 7,
                order_no = 2,
            },
            new
            {
                meal_id = 2,
                ingredient_id = 9,
                order_no = 1,
            },
            new
            {
                meal_id = 3,
                ingredient_id = 12,
                order_no = 1,
            },
            new
            {
                meal_id = 4,
                ingredient_id = 16,
                order_no = 1,
            },
        };

        //insert each packed one at a time
        foreach (var packed in packeds)
        {
            const string sql = """
                INSERT INTO packed (
                    meal_id,
                    ingredient_id,
                    order_no
                )
                SELECT
                    @meal_id,
                    @ingredient_id,
                    @order_no
                WHERE NOT EXISTS (
                    SELECT 1 FROM packed WHERE meal_id = @meal_id AND ingredient_id = @ingredient_id
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("meal_id", packed.meal_id);
            command.Parameters.AddWithValue("ingredient_id", packed.ingredient_id);
            command.Parameters.AddWithValue("order_no", packed.order_no);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("packeds seeded successfully.");
    }
}