using Npgsql;

namespace DummyData.CoreMock;
//seeder for the core database 
public static class CoreMockSeeder
{
    //create connection and transaction between the different async seed processes
    public static async Task SeedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction)
    {
        Console.WriteLine("Seeding CoreMock database...");

        // seeders
        await UserSeeder.SeedAsync(connection, transaction);
        await ClassroomSeeder.SeedAsync(connection, transaction);
        await ChildrenSeeder.SeedAsync(connection, transaction);
        await ChildrenRelationSeeder.SeedAsync(connection, transaction);
        await ImageSeeder.SeedAsync(connection, transaction);

        await Task.CompletedTask;
    }
}