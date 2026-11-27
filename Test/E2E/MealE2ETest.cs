using System.Net;
using System.Net.Http.Json;
using FoodplannerDataAccessSql;
using FoodplannerModels.Lunchbox;
using Npgsql;

namespace Test.E2E;

[Collection("E2EDatabase")]
public class MealE2ETest : IDisposable
{
    private readonly PostgreSQLConnectionFactory connectionFactory;
    private readonly HttpClient client;
    private const int UserId = 12345;

    public MealE2ETest(
        DatabaseFixture fixture,
        E2EWebApplicationFactory factory)
    {
        connectionFactory = fixture.ConnectionFactory;
        client = TestAuthentication.CreateAuthenticatedClient(
            factory, UserId);
    }

    public void Dispose()
    {
        client.Dispose();
    }
}