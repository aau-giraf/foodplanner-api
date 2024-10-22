using FoodplannerDataAccessSql;
using FoodplannerModels.Lunchbox;
using Dapper;

namespace testing;

public class MealRepositoryTest
{
    [Fact]
    public async void UnitTest_GetAllAsync()
    {
        PostgreSQLConnectionFactory _connectionFactory = DatabaseConnection.GetConnection();
        var sql = "SELECT * FROM meals";
        using var connection = _connectionFactory.Create();
        connection.Open();
        IEnumerable<Meal> result = await connection.QueryAsync<Meal>(sql);
        Assert.NotNull(result);
    }
}