using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Test.Integration.Seeder;

public class ProgramSeedRunnerTests
{
    private readonly IConfiguration configuration = new ConfigurationBuilder().Build();

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData("core", "CoreMockSeeder", "core_test")]
    [InlineData("CORE", "CoreMockSeeder", "core_test")]
    [InlineData("foodplanner", "FoodplannerSeeder", "foodplanner_test")]
    [InlineData("FOODPLANNER", "FoodplannerSeeder", "foodplanner_test")]
    public async Task Program_ShouldRunOnlyTheSelectedSeeder(string argument, string expectedSeeder, string expectedDatabase)
    {
        // Arrange
        Assert.True(DummyData.Program.TryParseTarget(new[] { argument }, out var target));
        var calls = new List<(string Seeder, string Database)>();

        // Act
        await DummyData.Program.RunSeedingAsync(target,configuration,GetTestSecret,
            (connectionString, seeder) =>
            {
                var connection = new NpgsqlConnectionStringBuilder(connectionString);
                calls.Add((seeder.Method.DeclaringType!.Name,connection.Database!));
                return Task.CompletedTask;
            });

        // Assert
        var call = Assert.Single(calls);
        Assert.Equal(expectedSeeder,call.Seeder);
        Assert.Equal(expectedDatabase,call.Database);
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData("all")]
    [InlineData(null)]
    public async Task Program_ShouldRunBothSeedersInOrder(string? argument)
    {
        // Arrange
        string[] args = argument == null ? Array.Empty<string>() : new[] { argument };
        Assert.True(DummyData.Program.TryParseTarget(args,out var target));
        var calls = new List<(string Seeder, string Database)>();

        // Act
        await DummyData.Program.RunSeedingAsync(target,configuration,GetTestSecret,
            (connectionString, seeder) =>
            {
                var connection = new NpgsqlConnectionStringBuilder(connectionString);
                calls.Add((seeder.Method.DeclaringType!.Name,connection.Database!));
                return Task.CompletedTask;
            });

        // Assert
        Assert.Collection(calls,
            core =>
            {
                Assert.Equal("CoreMockSeeder",core.Seeder);
                Assert.Equal("core_test",core.Database);
            },
            foodplanner =>
            {
                Assert.Equal("FoodplannerSeeder",foodplanner.Seeder);
                Assert.Equal("foodplanner_test",foodplanner.Database);
            });
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData("invalid")]
    [InlineData("unsupported")]
    public async Task Program_ShouldRejectInvalidTarget(string target)
    {
        // Act
        var result = await RunDummyDataAsync("Development",target);

        // Assert
        Assert.Equal(0,result.ExitCode);
        Assert.Contains("Invalid argument.",result.Output);
        Assert.DoesNotContain("Dummy data generation completed.",result.Output);
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData("Production", "core")]
    [InlineData("Staging", "foodplanner")]
    [InlineData("Production", "all")]
    public async Task Program_ShouldRejectNonDevelopmentEnvironment(string environment, string target)
    {
        // Act
        var result = await RunDummyDataAsync(environment,target);

        // Assert
        Assert.Equal(0,result.ExitCode);
        Assert.Contains("Dummy data can only run in Development.",result.Output);
        Assert.DoesNotContain("Dummy data generation completed.",result.Output);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Program_ShouldRejectNonDevelopmentEnvironmentWithDefaultTarget()
    {
        // Act
        var result = await RunDummyDataAsync("Production");

        // Assert
        Assert.Equal(0,result.ExitCode);
        Assert.Contains("Dummy data can only run in Development.",result.Output);
        Assert.DoesNotContain("Dummy data generation completed.",result.Output);
    }

    private static string GetTestSecret(string key) => key switch
    {
        "CORE_DB_HOST" or "DB_HOST" => "127.0.0.1",
        "CORE_DB_PORT" or "DB_PORT" => "5432",
        "CORE_DB_NAME" => "core_test",
        "DB_NAME" => "foodplanner_test",
        "CORE_DB_USER" or "DB_USER" => "test_user",
        "CORE_DB_PASS" or "DB_PASS" => "test_password",
        _ => throw new ArgumentException($"Unexpected secret key: {key}")
    };

    private static async Task<(int ExitCode, string Output)> RunDummyDataAsync(string environment, params string[] args)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        string? projectPath = null;
        while (directory != null)
        {
            string candidate = Path.Combine(directory.FullName,"Development","DummyData","DummyData.csproj");
            if (File.Exists(candidate))
            {
                projectPath = candidate;
                break;
            }
            directory = directory.Parent;
        }

        if (projectPath == null)
            throw new DirectoryNotFoundException("Could not locate Development/DummyData/DummyData.csproj.");

        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(projectPath)))!,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        start.ArgumentList.Add("run");
        start.ArgumentList.Add("--no-build");
        start.ArgumentList.Add("--no-launch-profile");
        start.ArgumentList.Add("--project");
        start.ArgumentList.Add(projectPath);
        start.ArgumentList.Add("--");
        foreach (var arg in args)
            start.ArgumentList.Add(arg);

        start.Environment["DOTNET_ENVIRONMENT"] = environment;
        start.Environment["ASPNETCORE_ENVIRONMENT"] = environment;

        using var process = new Process { StartInfo = start };
        if (!process.Start())
            throw new InvalidOperationException("Failed to launch DummyData.");

        Task<string> output = process.StandardOutput.ReadToEndAsync();
        Task<string> errors = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);
            throw new TimeoutException("DummyData did not complete within 90 seconds.");
        }

        return (process.ExitCode, (await output) + Environment.NewLine + (await errors));
    }
}
