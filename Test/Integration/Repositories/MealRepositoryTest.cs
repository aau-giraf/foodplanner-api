using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;

using Npgsql;
using Xunit;

using Test.Integration;

[Collection("IntegrationDatabase")]
public class MealRepositorytests
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly MealsRepository repository;

    public MealRepositorytests(DatabaseFixture fixture)
    {
        connectionFactory = fixture.ConnectionFactory;

        repository = new MealsRepository(
            new DapperExecutor(connectionFactory));
    }


    
}