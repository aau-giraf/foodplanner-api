using Npgsql;
using NpgsqlTypes;

namespace DummyData.CoreMock;

public static class ImageSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding images...");

        // Dummy lunchbox images
        var lunchboxImages = new[]
        {
            new
            {
                user_id = 1,
                image_name = "Test's Madpakke",
                folder = "Lunchboxes",
                file_name = "lunchbox1.png"
            },
            new
            {
                user_id = 2,
                image_name = "Frederik's Madpakke",
                folder = "Lunchboxes",
                file_name = "lunchbox2.png"
            },
            new
            {
                user_id = 2,
                image_name = "Sara's Madpakke",
                folder = "Lunchboxes",
                file_name = "lunchbox3.png"
            },
            new
            {
                user_id = 2,
                image_name = "Sebastian's Madpakke",
                folder = "Lunchboxes",
                file_name = "lunchbox4.png"
            }
        };

        // Dummy ingredient images
        var ingredientImages = new[]
        {
            new
            {
                user_id = 1,
                image_name = "Pasta Plader",
                folder = "Ingredients",
                file_name = "Ingrediens1.png"
            },
            new
            {
                user_id = 1,
                image_name = "Tomat Sauce",
                folder = "Ingredients",
                file_name = "Ingrediens2.png"
            },
            new
            {
                user_id = 1,
                image_name = "Tomat",
                folder = "Ingredients",
                file_name = "Ingrediens3.png"
            },
            new
            {
                user_id = 1,
                image_name = "Hakkede Oksekød",
                folder = "Ingredients",
                file_name = "Ingrediens4.png"
            },
            new
            {
                user_id = 1,
                image_name = "Revet Ost",
                folder = "Ingredients",
                file_name = "Ingrediens5.png"
            },
            new
            {
                user_id = 1,
                image_name = "Æble Juice Brik",
                folder = "Ingredients",
                file_name = "Ingrediens6.png"
            },
            new
            {
                user_id = 1,
                image_name = "Æble",
                folder = "Ingredients",
                file_name = "Ingrediens7.png"
            },
            new
            {
                user_id = 2,
                image_name = "Burger Bøf",
                folder = "Ingredients",
                file_name = "Ingrediens8.png"
            },
            new
            {
                user_id = 2,
                image_name = "Burger Bolle Bund",
                folder = "Ingredients",
                file_name = "Ingrediens9.png"
            },
            new
            {
                user_id = 2,
                image_name = "Ris",
                folder = "Ingredients",
                file_name = "Ingrediens10.png"
            },
            new
            {
                user_id = 2,
                image_name = "Agurk",
                folder = "Ingredients",
                file_name = "Ingrediens11.png"
            },
            new
            {
                user_id = 2,
                image_name = "Tang",
                folder = "Ingredients",
                file_name = "Ingrediens12.png"
            },
            new
            {
                user_id = 2,
                image_name = "Lauerbærblad",
                folder = "Ingredients",
                file_name = "Ingrediens13.png"
            }
        };

        // Combine images
        var images = lunchboxImages.Concat(ingredientImages);

        //insert each image one at a time
        foreach (var image in images)
        {
            // Get image file path
            var imagePath = Path.Combine(
                AppContext.BaseDirectory,
                "CoreMock",
                "Images",
                image.folder,
                image.file_name
            );

            // Read image as byte array
            byte[] imageData = await File.ReadAllBytesAsync(imagePath);

            // Get image size in bytes
            long size = imageData.LongLength;

            const string sql = """
                INSERT INTO food_image (
                    user_id,
                    image_name,
                    image_file_type,
                    size,
                    image_data
                )
                SELECT
                    @user_id,
                    @image_name,
                    @image_file_type,
                    @size,
                    @image_data
                WHERE NOT EXISTS (
                    SELECT 1 FROM food_image
                    WHERE user_id = @user_id
                    AND image_name = @image_name
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("user_id", image.user_id);
            command.Parameters.AddWithValue("image_name", image.image_name);
            command.Parameters.AddWithValue("image_file_type", "PNG");
            command.Parameters.AddWithValue("size", size);
            command.Parameters.Add("image_data", NpgsqlDbType.Bytea).Value = imageData;

            await command.ExecuteNonQueryAsync();

            Console.WriteLine($"Seeded image: {image.image_name}");
        }

        Console.WriteLine("Images seeded successfully.");
    }
}