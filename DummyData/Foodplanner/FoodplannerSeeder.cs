using Npgsql;

namespace DummyData.Foodplanner;
//seeder for the foodplanner database 
public static class FoodplannerSeeder
{
    //create connection and transaction between the different async seed processes
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding Foodplanner database...");

        // Foodplanner seeders
        await MealSeeder.SeedAsync(connection, transaction);
        await IngredientSeeder.SeedAsync(connection, transaction);
        await SubingredientSeeder.SeedAsync(connection, transaction);
        await PackedSeeder.SeedAsync(connection, transaction);
        await Task.CompletedTask;
    }
}