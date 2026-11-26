using System.Net;
using FoodplannerDataAccessSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Test.E2E;

[Collection("E2EDatabase")]
public class DatabaseConnectionTests
{
    private readonly E2EWebApplicationFactory factory;

    public DatabaseConnectionTests(E2EWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task Api_ShouldConnectToTestDatabase()
    {
        // Arrange
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions{BaseAddress = new Uri("https://localhost"),AllowAutoRedirect = false});

        var environment = factory.Services
            .GetRequiredService<IWebHostEnvironment>();

        var connectionFactory = factory.Services
            .GetRequiredService<PostgreSQLConnectionFactory>();

        // Act
        var response = await client.GetAsync("/test-db-connection");

        // Assert
        Assert.Equal("Testing", environment.EnvironmentName);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync();

        Assert.Equal("localhost", connection.Host);
        Assert.Equal(7656, connection.Port);

        await using var command = new NpgsqlCommand(
            "SELECT current_database()", connection);

        var databaseName = await command.ExecuteScalarAsync();

        Assert.Equal("giraf_test", databaseName);
    }
}