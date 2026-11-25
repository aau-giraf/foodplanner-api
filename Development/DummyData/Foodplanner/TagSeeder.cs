using Npgsql;

namespace DummyData.Foodplanner;

public static class TagSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding tags...");

        // Dummy tags
        var tags = new[]
        {
            new
            {
                ingredient_id = 8, //æble
                category_id = 1, //Frugt og Grønt
                
            },
            new
            {
                ingredient_id = 12, //agurk sushi
                category_id = 1, //Frugt og Grønt
                
            },
            new
            {
                ingredient_id = 10, //bøf
                category_id = 2, //Pålæg
                
            },
            new
            {
                ingredient_id = 6, //Revet ost
                category_id = 3, //Mejeri
                
            },
            new
            {
                ingredient_id = 11, //burger bolle
                category_id = 4, //Brød
                
            },
            new
            {
                ingredient_id = 7, //Æble juice
                category_id = 5, //Drikkevarer
                
            },
            new
            {
                ingredient_id = 13, //ris
                category_id = 6, //Andet
                
            },
            new
            {
                ingredient_id = 5, //Oksekød
                category_id = 6, //Andet
                
            },
            new
            {
                ingredient_id = 16, // Lauerbærblad
                category_id = 6, // Andet
            },
            new
            {
                ingredient_id = 5, //hakket oksekød
                category_id = 7, //Halal
                
            },
            new
            {
                ingredient_id = 10, //bøf
                category_id = 7, //Halal
                
            },
            new
            {
                ingredient_id = 12, //agurk sushi
                category_id = 8, //Skiver
                
            },
            new
            {
                ingredient_id = 5, //Hakket oksekød
                category_id = 9, //Ko
                
            },
            new
            {
                ingredient_id = 10, //Bøf
                category_id = 9, //Ko
                
            },
            new
            {
                ingredient_id = 13, //Ris
                category_id = 10, //Mushy
                
            },
            new
            {
                ingredient_id = 16, //lauerbærblad
                category_id = 11, //Fermenteret
                
            },
        };

        //insert each tag one at a time
        foreach (var tag in tags)
        {
            const string sql = """
                INSERT INTO tags (
                    ingredient_id,
                    category_id
                )
                SELECT
                    @ingredient_id,
                    @category_id
                WHERE NOT EXISTS (
                    SELECT 1 FROM tags WHERE ingredient_id = @ingredient_id and category_id = @category_id
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("ingredient_id", tag.ingredient_id);
            command.Parameters.AddWithValue("category_id", tag.category_id);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("tags seeded successfully.");
    }
}