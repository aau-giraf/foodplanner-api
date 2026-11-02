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
        await Task.CompletedTask;
    }
}