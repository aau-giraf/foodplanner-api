using FoodplannerServices.Secret;
using Microsoft.Extensions.Configuration;
using Moq;

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
        var exception = Record.Exception(() => SecretsLoader.Configure(config, environmentString));

        // Assert
        Assert.Null(exception);
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
        var exception = Record.Exception(() => SecretsLoader.Configure(emptyConfig, environmentString));

        // Assert
        Assert.Null(exception);

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

        // Act + Assert
        Assert.Throws<ApplicationException>(() => SecretsLoader.Configure(emptyConfig, environmentString));
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

        SecretsLoader.Configure(config, environmentString);

        var secretKey = "ClientId";

        // Act
        var result = SecretsLoader.GetSecret(secretKey);

        // Assert
        var expectedSecret = "TestClientId";
        Assert.Equal(expectedSecret, result);
    }

    [Fact]
    public void GetSecret_InvalidSecretKey_ReturnsNull()
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

        SecretsLoader.Configure(config, environmentString);

        var invalidSecretKey = "InvalidSecretKey";

        // Act
        var result = SecretsLoader.GetSecret(invalidSecretKey);

        // Assert
        Assert.Null(result);
    }
}
