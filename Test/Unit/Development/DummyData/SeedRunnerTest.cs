
using Xunit;
using Npgsql;

namespace Test.Unit.DummyData;

public class SeedRunnerTest
{
    // Test that the correct databases are selected based on the target
    [Theory]
    [InlineData("all", true, true)]
    [InlineData("core", true, false)]
    [InlineData("foodplanner", false, true)]
    [InlineData("invalid", false, false)]
    [Trait("Category", "Unit")]
    public void ShouldSeed_SelectsCorrectDatabase(
        string target,
        bool expectedCore,
        bool expectedFoodplanner)
    {
        // Act
        var coreResult = global::DummyData.SeedRunner.ShouldSeed(
            target, "core");

        var foodplannerResult = global::DummyData.SeedRunner.ShouldSeed(
            target, "foodplanner");

        // Assert
        Assert.Equal(expectedCore, coreResult);
        Assert.Equal(expectedFoodplanner, foodplannerResult);
    }

    // Test that connection strings are built with the correct database settings
    [Theory]
    [InlineData("CORE_DB", "localhost", 7655, "giraf_core_mock")]
    [InlineData("DB", "localhost", 7654, "giraf")]
    [Trait("Category", "Unit")]
    public void BuildConnectionString_UsesCorrectConfiguration(
        string prefix,
        string host,
        int port,
        string database)
    {
        // Arrange
        var secrets = new Dictionary<string, string>
        {
            [$"{prefix}_HOST"] = host,
            [$"{prefix}_PORT"] = port.ToString(),
            [$"{prefix}_NAME"] = database,
            [$"{prefix}_USER"] = "postgres",
            [$"{prefix}_PASS"] = "postgres"
        };

        // Act
        var connectionString =
            global::DummyData.SeedRunner.BuildConnectionString(
                key => secrets[key],
                prefix
            );

        var result = new NpgsqlConnectionStringBuilder(connectionString);

        // Assert
        Assert.Equal(host, result.Host);
        Assert.Equal(port, result.Port);
        Assert.Equal(database, result.Database);
        Assert.Equal("postgres", result.Username);
        Assert.Equal("postgres", result.Password);
    }

    // Test that an invalid port number throws a FormatException
    [Fact]
    [Trait("Category", "Unit")]
    public void BuildConnectionString_InvalidPort_ThrowsException()
    {
        // Arrange
        var secrets = new Dictionary<string, string>
        {
            ["DB_HOST"] = "localhost",
            ["DB_PORT"] = "invalid",
            ["DB_NAME"] = "giraf",
            ["DB_USER"] = "postgres",
            ["DB_PASS"] = "postgres"
        };

        // Act & Assert
        Assert.Throws<FormatException>(() =>
            global::DummyData.SeedRunner.BuildConnectionString(
                key => secrets[key],
                "DB"
            )
        );
    }

    // Test that missing configuration values throw an exception
    [Fact]
    [Trait("Category", "Unit")]
    public void BuildConnectionString_MissingSecret_ThrowsException()
    {
        // Arrange
        var secrets = new Dictionary<string, string>
        {
            ["DB_HOST"] = "localhost",
            ["DB_PORT"] = "7654",
            ["DB_NAME"] = "giraf",
            ["DB_USER"] = "postgres"
            // DB_PASS is intentionally missing
        };

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            global::DummyData.SeedRunner.BuildConnectionString(
                key => secrets[key],
                "DB"
            )
        );
    }
}
