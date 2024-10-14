using Infisical.Sdk;

namespace FoodplannerApi;

public static class SecretsLoader
{
    private record Configuration(string environmentSlug, string workspaceId, InfisicalClient Client);
    private static Configuration _configuration = null!;

    /// Method <c>Configure</c> initialises the SecretsLoader. It reads from appsettings.{environment}.json,
    /// but will fall back to environment variables. <br />
    /// Run this before reading secrets.
    public static void Configure(IConfiguration config, string environment)
    {
        var clientId = config.GetValue<string>("Infisical:ClientId") ?? Environment.GetEnvironmentVariable("CLIENT_ID");
        var clientSecret = config.GetValue<string>("Infisical:ClientSecret") ?? Environment.GetEnvironmentVariable("CLIENT_SECRET");
        var workspaceId = config.GetValue<string>("Infisical:Workspace") ?? Environment.GetEnvironmentVariable("WORKSPACE");
        if (string.IsNullOrWhiteSpace(clientId) || 
            string.IsNullOrWhiteSpace(clientSecret) || 
            string.IsNullOrWhiteSpace(workspaceId))
        {
            throw new ApplicationException("Missing environment variables");
        }
        
        var settings = new ClientSettings
        {
            Auth = new AuthenticationOptions
            {
                UniversalAuth = new UniversalAuthMethod
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                }
            }
        };
        
        _configuration = new Configuration(MapEnvironmentToSlug(environment), workspaceId, new InfisicalClient(settings));
    }

    public static string GetSecret(string secretName, string path = "/")
    {
        if (_configuration == null)
        {
            throw new ApplicationException($"Unable to load secret: {secretName}. SecretsLoader must be configured before usage");
        }
        var getSecretOptions = new GetSecretOptions
        {
            Path = path,
            SecretName = secretName,
            ProjectId = _configuration.workspaceId,
            Environment = _configuration.environmentSlug,
        };
        
        return _configuration.Client.GetSecret(getSecretOptions).SecretValue;
    }

    private static string MapEnvironmentToSlug(string environment) => environment switch
    {
        "Development" => "dev",
        "Staging" => "staging",
        "Production" => "prod",
        _ => environment
    };
}