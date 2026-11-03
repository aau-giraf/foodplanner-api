using DummyData;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

//check target command, if its less then 1 set it to all, else set all lovercase
var target = args.Length > 0
    ? args[0].ToLowerInvariant()
    : "all";

//if target is all, core or foodplanner approve, else print invalid argument
if (target is not ("all" or "core" or "foodplanner"))
{
    Console.WriteLine("Invalid argument.");
    return;
}

// Load the same configuration as FoodplannerApi
var builder = Host.CreateApplicationBuilder(
    new HostApplicationBuilderSettings
    {
        Args = Array.Empty<string>(),
        ContentRootPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "FoodplannerApi"
        )
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
//await seed runners
await SeedRunner.RunAsync(target, builder.Configuration);