using Npgsql;

namespace DummyData.Foodplanner;

public static class CategorySeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding categories...");

        // Dummy categories
        var categories = new[]
        {
            new
            {
                name = "Frugt og Grønt",
                main = true, 
            },
            new
            {
                name = "Pålæg",
                main = true,
            },
            new
            {
                name = "Mejeri",
                main = true,
            },
            new
            {
                name = "Brød",
                main = true,
            },
            new
            {
                name = "Drikkevarer",
                main = true,
            },
            new
            {
                name = "Andet",
                main = true,
            },
            new
            {
                name = "Halal",
                main = false,
            },
            new
            {
                name = "Skiver",
                main = false,
            },
            new
            {
                name = "Ko",
                main = false,
            },
            new
            {
                name = "Mushy",
                main = false,
            },
            new
            {
                name = "Fermenteret",
                main = false,
            }
        };

        //insert each category one at a time
        foreach (var category in categories)
        {
            const string sql = """
                INSERT INTO categories (
                    name,
                    main
                )
                SELECT
                    @name,
                    @main
                WHERE NOT EXISTS (
                    SELECT 1 FROM categories WHERE name = @name
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("name", category.name);
            command.Parameters.AddWithValue("main", category.main);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("categories seeded successfully.");
    }
}