using FoodplannerServices.Secret;
using Infisical.Sdk;
using Microsoft.Extensions.Configuration;

namespace Test.Secret;

public class SecretsLoaderTests
{
    [Fact]
    public void Configure_ConfigContainsStrings_DoesNotThrow()
    {
        // Arrange
        var ClientIdKey = "Infisical:ClientId";
        var ClientSecretKey = "Infisical:ClientSecret";
        var WorkspaceKey = "Infisical:Workspace";

        var ClientIdValue = "TestClientId";
        var ClientSecretValue = "TestClientSecret";
        var WorkspaceValue = "TestWorkspace";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {ClientIdKey, ClientIdValue},
                {ClientSecretKey, ClientSecretValue},
                {WorkspaceKey, WorkspaceValue}
            })
            .Build();

        var environmentString = "TestEnvironment";

        // Act
        var secretLoader = new SecretsLoader(config, environmentString);

        // Assert
        Assert.NotNull(secretLoader);
    }

    [Fact]
    public void Configure_EnvironmentContainsStrings_DoesNotThrow()
    {
        // Arrange
        var ClientIdKey = "CLIENT_ID";
        var ClientSecretKey = "CLIENT_SECRET";
        var WorkspaceKey = "WORKSPACE";

        var ClientIdValue = "TestClientId";
        var ClientSecretValue = "TestClientSecret";
        var WorkspaceValue = "TestWorkspace";

        var emptyConfig = new ConfigurationBuilder().Build();

        Environment.SetEnvironmentVariable(ClientIdKey, ClientIdValue);
        Environment.SetEnvironmentVariable(ClientSecretKey, ClientSecretValue);
        Environment.SetEnvironmentVariable(WorkspaceKey, WorkspaceValue);

        var environmentString = "TestEnvironment";

        // Act
        var secretLoader = new SecretsLoader(emptyConfig, environmentString);

        // Assert
        Assert.NotNull(secretLoader);

        // Cleanup
        Environment.SetEnvironmentVariable(ClientIdKey, null);
        Environment.SetEnvironmentVariable(ClientSecretKey, null);
        Environment.SetEnvironmentVariable(WorkspaceKey, null);
    }

    [Fact]
    public void Configure_NoConfigOrEnvironmentStrings_ThrowsApplicationException()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();

        var environmentString = "TestEnvironment";

        // Act
        var exception = Record.Exception(() => new SecretsLoader(emptyConfig, environmentString));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ApplicationException>(exception);
    }

    [Fact]
    public void GetSecret_ValidSecretKey_ReturnsSecret()
    {
        // Arrange
        var ClientIdKey = "Infisical:ClientId";
        var ClientSecretKey = "Infisical:ClientSecret";
        var WorkspaceKey = "Infisical:Workspace";

        var ClientIdValue = "TestClientId";
        var ClientSecretValue = "TestClientSecret";
        var WorkspaceValue = "TestWorkspace";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {ClientIdKey, ClientIdValue},
                {ClientSecretKey, ClientSecretValue},
                {WorkspaceKey, WorkspaceValue}
            })
            .Build();

        var environmentString = "TestEnvironment";

        var secretKey = "ClientId";

        var sut = new SecretsLoader(config, environmentString);

        // Act
        var result = sut.GetSecret(secretKey);

        // Assert
        var expectedSecret = "TestClientId";
        Assert.Equal(expectedSecret, result);
    }

    [Fact]
    public void GetSecret_InvalidSecretKey_ThrowsInfisicalException()
    {
        // Arrange
        var ClientIdKey = "Infisical:ClientId";
        var ClientSecretKey = "Infisical:ClientSecret";
        var WorkspaceKey = "Infisical:Workspace";

        var ClientIdValue = "TestClientId";
        var ClientSecretValue = "TestClientSecret";
        var WorkspaceValue = "TestWorkspace";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {ClientIdKey, ClientIdValue},
                {ClientSecretKey, ClientSecretValue},
                {WorkspaceKey, WorkspaceValue}
            })
            .Build();

        var environmentString = "TestEnvironment";

        var sut = new SecretsLoader(config, environmentString);

        var invalidSecretKey = "InvalidSecretKey";

        // Act
        var exception = Record.Exception(() => sut.GetSecret(invalidSecretKey));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InfisicalException>(exception);
    }
}
