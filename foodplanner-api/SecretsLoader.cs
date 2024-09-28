using Infisical.Sdk;

namespace foodplanner_api;

public static class SecretsLoader
{
    private static string? _environment;
    private static string? _workspaceId;
    private static InfisicalClient? _infisicalClient;

    public static void Configure(IConfiguration config, string environment)
    {
        var clientId = config.GetValue<string>("Infisical:ClientId");
        var clientSecret = config.GetValue<string>("Infisical:ClientSecret");
        var workspaceId = config.GetValue<string>("Infisical:Workspace");
        if (string.IsNullOrWhiteSpace(clientId) || 
            string.IsNullOrWhiteSpace(clientSecret) || 
            string.IsNullOrWhiteSpace(workspaceId))
        {
            throw new ApplicationException("Missing environment variables");
        }

        _environment = MapEnvironment(environment);
        _workspaceId = workspaceId;
        
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
        
        _infisicalClient = new InfisicalClient(settings);
    }

    public static string GetSecret(string secretName)
    {
        if (_environment == null || _workspaceId == null || _infisicalClient == null)
        {
            throw new ApplicationException($"Unable to load secret: {secretName}. SecretsLoader must be configured before usage");
        }
        var getSecretOptions = new GetSecretOptions
        {
            SecretName = secretName,
            ProjectId = _workspaceId,
            Environment = _environment,
        };
        
        return _infisicalClient.GetSecret(getSecretOptions).SecretValue;
    }

    private static string MapEnvironment(string environment) => environment switch
    {
        "Development" => "dev",
        "Staging" => "staging",
        "Production" => "prod",
        _ => environment
    };
}