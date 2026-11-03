
using DummyData;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

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
        ContentRootPath = FindApiDirectory()
    }
);

// Add environment variables for Infisical
builder.Configuration.AddEnvironmentVariables(prefix: "INFISICAL_");

// Only allow dummy data in Development
if (!builder.Environment.IsDevelopment())
{
    Console.WriteLine("Dummy data can only run in Development.");
    return;
}

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

// Run selected seeders
await SeedRunner.RunAsync(target, builder.Configuration);

// Find FoodplannerApi directory regardless of execution location
static string FindApiDirectory()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);

    while (directory != null)
    {
        var apiPath = Path.Combine(
            directory.FullName,
            "FoodplannerApi"
        );

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

namespace DummyData
{
    public partial class Program
    {
        // Get target command, defaulting to "all"
        public static bool TryParseTarget(string[] args, out string target)
        {
            target = args.Length > 0
                ? args[0].ToLowerInvariant()
                : "all";

            return target is "all" or "core" or "foodplanner";
        }
    }
}