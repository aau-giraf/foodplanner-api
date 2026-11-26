using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Test.E2E;

public class E2EWebApplicationFactory : WebApplicationFactory<ProgramTest>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use the testing environment when starting the API
        builder.UseEnvironment("Testing");
    }
}