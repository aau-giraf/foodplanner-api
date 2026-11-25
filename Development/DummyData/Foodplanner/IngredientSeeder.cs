using Npgsql;

namespace DummyData.Foodplanner;

public static class IngredientSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding ingredients...");

        // Dummy ingredients
        var ingredients = new[]
        {
            new
            {
                name = "Lasagne",
                image_id = 5,
                user_id = 1,
                is_stock_img = false,
            },
            new
            {
                name = "Pasta Plader",
                image_id = 6,
                user_id = 1,
                is_stock_img = false,
            },
            new
            {
                name = "Tomat Sauce",
                image_id = 7,
                user_id = 1,
                is_stock_img = false,
            },
            new
            {
                name = "Tomat",
                image_id = 8,
                user_id = 1,
                is_stock_img = false,
            },
            new
            {
                name = "Hakkede Oksekød",
                image_id = 9,
                user_id = 1,
                is_stock_img = true,
            },
            new
            {
                name = "Revet Ost",
                image_id = 10,
                user_id = 1,
                is_stock_img = true,
            },
            new
            {
                name = "Æble Juice Brik",
                image_id = 11,
                user_id = 1,
                is_stock_img = true,
            },
            new
            {
                name = "Æble",
                image_id = 12,
                user_id = 1,
                is_stock_img = true,
            },
            new
            {
                name = "Burger",
                image_id = 13,
                user_id = 2,
                is_stock_img = false,
            },
            new
            {
                name = "Burger Bøf",
                image_id = 14,
                user_id = 2,
                is_stock_img = true,
            },
            new
            {
                name = "Burger Bolle Bund",
                image_id = 15,
                user_id = 2,
                is_stock_img = true,
            },
            new
            {
                name = "Agurk Sushi",
                image_id = 16,
                user_id = 2,
                is_stock_img = true,
            },
            new
            {
                name = "Ris",
                image_id = 17,
                user_id = 2,
                is_stock_img = true,
            },
            new
            {
                name = "Agurk",
                image_id = 18,
                user_id = 2,
                is_stock_img = false,
            },
            new
            {
                name = "Tang",
                image_id = 19,
                user_id = 2,
                is_stock_img = false,
            },
            new
            {
                name = "Lauerbærblad",
                image_id = 20,
                user_id = 2,
                is_stock_img = true,
            },
        };

        //insert each ingredient one at a time
        foreach (var ingredient in ingredients)
        {
            const string sql = """
                INSERT INTO ingredients (
                    name,
                    image_id,
                    user_id,
                    is_stock_img
                )
                SELECT
                    @name,
                    @image_id,
                    @user_id,
                    @is_stock_img
                WHERE NOT EXISTS (
                    SELECT 1 FROM ingredients WHERE name = @name
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("name", ingredient.name);
            command.Parameters.AddWithValue("image_id", ingredient.image_id);
            command.Parameters.AddWithValue("user_id", ingredient.user_id);
            command.Parameters.AddWithValue("is_stock_img", ingredient.is_stock_img);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("ingredients seeded successfully.");
    }
}