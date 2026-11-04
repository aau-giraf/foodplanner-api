
using DummyData;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

// Validate target command, defaulting to "all"
if (!DummyData.Program.TryParseTarget(args, out var target))
{
    Console.WriteLine("Invalid argument.");
    return;
}

// Load the same configuration as FoodplannerApi
var builder = Host.CreateApplicationBuilder(
    new HostApplicationBuilderSettings
    {
        Args = Array.Empty<string>(),
        ContentRootPath = DummyData.Program.FindApiDirectory(AppContext.BaseDirectory)
    }
);

// Add environment variables for Infisical
builder.Configuration.AddEnvironmentVariables(prefix: "INFISICAL_");

// Only allow dummy data generation in Development
if (!DummyData.Program.IsAllowedEnvironment(
    builder.Environment.EnvironmentName))
{
    Console.WriteLine("Dummy data can only run in Development.");
    return;
}

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

// Run selected seeders
await SeedRunner.RunAsync(target, builder.Configuration);


namespace DummyData
{
    public partial class Program
    {
        // Get target command, defaulting to "all" if no arguments are provided
        // Returns true if the target is valid, otherwise false
        public static bool TryParseTarget(string[] args, out string target)
        {
            target = args.Length > 0 ? args[0].ToLowerInvariant() : "all";

            return target is "all" or "core" or "foodplanner";
        }

        // Find the FoodplannerApi directory by searching parent directories
        // Allows the application to run regardless of execution location
        public static string FindApiDirectory(string startDirectory)
        {
            var directory = new DirectoryInfo(startDirectory);

            while (directory != null)
            {
                var apiPath = Path.Combine(directory.FullName,"FoodplannerApi");

                if (Directory.Exists(apiPath))
                {
                    return apiPath;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException(
                "Could not locate FoodplannerApi directory."
            );
        }

        // Check if the current environment is Development
        // Returns true for Development, otherwise false
        public static bool IsAllowedEnvironment(string? environment)
        {
            return string.Equals(
                environment,
                Environments.Development,
                StringComparison.OrdinalIgnoreCase
            );
        }
    }
}
