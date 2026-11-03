using Npgsql;

namespace DummyData.Foodplanner;

public static class MealSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding meals...");

        // Dummy meals
        var meals = new[]
        {
            new
            {
                name = "Test's Madpakke",
                date = DateTime.Today.AddDays(7).ToString(),
                image_id = 1,
                user_id = 1,
            },
            new
            {
                name = "Frederik's Madpakke",
                date = DateTime.Today.AddDays(7).ToString(),
                image_id = 2,
                user_id = 2,
            },
            new
            {
                name = "Sebastian's Madpakke",
                date = DateTime.Today.AddDays(7).ToString(),
                image_id = 3,
                user_id = 2,
            },
            new
            {
                name = "Sara's Madpakke",
                date = DateTime.Today.AddDays(7).ToString(),
                image_id = 4,
                user_id = 2,
            },
        };

        //insert each meal one at a time
        foreach (var meal in meals)
        {
            const string sql = """
                INSERT INTO meals (
                    name,
                    date,
                    image_id,
                    user_id
                )
                SELECT
                    @name,
                    @date,
                    @image_id,
                    @user_id
                WHERE NOT EXISTS (
                    SELECT 1 FROM meals WHERE name = @name
                );
                """;

            await using var command =
                new NpgsqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue("name", meal.name);
            command.Parameters.AddWithValue("date", meal.date);
            command.Parameters.AddWithValue("image_id", meal.image_id);
            command.Parameters.AddWithValue("user_id", meal.user_id);

            await command.ExecuteNonQueryAsync();
        }

        Console.WriteLine("meals seeded successfully.");
    }
}