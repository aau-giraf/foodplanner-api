using Npgsql;

namespace DummyData.Foodplanner;

public static class FoodplannerSeeder
{
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding Foodplanner database...");

        // Add individual seeders here
        await MealSeeder.SeedAsync(connection, transaction);
        await IngredientSeeder.SeedAsync(connection, transaction);
        //await SubingredientSeeder.SeedAsync(connection, transaction);
        //await PackedSeeder.SeedAsync(connection, transaction);
        await Task.CompletedTask;
    }
}