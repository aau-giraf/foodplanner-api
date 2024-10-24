using FoodplannerApi;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerDataAccessSql;
using FoodplannerModels.Lunchbox;
using Npgsql;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Infisical.Sdk;

namespace testing;

public class MealRepositoryTest
{
    // [Fact]
    // public async void UnitTest_GetAllAsync()
    // {
    //     var builder = WebApplication.CreateBuilder();
    //     builder.Configuration.AddEnvironmentVariables(prefix: "INFISICAL_");
    //     SecretsLoader.Configure(builder.Configuration, builder.Environment.EnvironmentName);
    //     var host = SecretsLoader.GetSecret("DB_HOST", "/SW-5-10/");
    //     var port = SecretsLoader.GetSecret("DB_PORT", "/SW-5-10/");
    //     var database = SecretsLoader.GetSecret("DB_NAME", "/SW-5-10/");
    //     var username = SecretsLoader.GetSecret("DB_USER", "/SW-5-10/");
    //     var password = SecretsLoader.GetSecret("DB_PASS", "/SW-5-10/");
    //     PostgreSQLConnectionFactory _connectionFactory = new PostgreSQLConnectionFactory(host, port, database, username, password);
    //     var sql = "SELECT * FROM meals";
    //     using var connection = _connectionFactory.Create();
    //     connection.Open();
    //     IEnumerable<Meal> result = await connection.QueryAsync<Meal>(sql);
    //     Assert.NotNull(result);
    // }
}