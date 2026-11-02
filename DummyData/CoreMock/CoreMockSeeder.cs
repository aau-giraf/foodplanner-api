using Npgsql;

namespace DummyData.CoreMock;

public static class CoreMockSeeder
{
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