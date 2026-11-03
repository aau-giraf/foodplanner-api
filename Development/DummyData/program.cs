
using DummyData;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

// Get target command, defaulting to "all"
var target = args.Length > 0
    ? args[0].ToLowerInvariant()
    : "all";

// Validate target command
if (target is not ("all" or "core" or "foodplanner"))
{
    Console.WriteLine("Invalid argument. Use: all, core or foodplanner.");
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
