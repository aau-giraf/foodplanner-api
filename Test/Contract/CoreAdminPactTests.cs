using System.Net;
using FoodplannerModels.Account;
using PactNet;
using PactNet.Matchers;

namespace Test.Contract;

public class CoreAdminPactTests
{
    private readonly IPactBuilderV4 _pact;

    public CoreAdminPactTests()
    {
        var config = new PactConfig
        {
            PactDir = Path.Combine(AppContext.BaseDirectory, "../../../../contracts"),
        };

        _pact = Pact.V4(
            "foodplanner-api",
            "core",
            config).WithHttpInteractions();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetUserByIdAsync_satisfies_the_contract_with_core()
    {
        _pact
            .UponReceiving("a request for a specific user")
            .Given("that user exists")
            .WithRequest(HttpMethod.Get, "/legacy/foodplanner/api/Admin/Get/1")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {

            });

        await _pact.VerifyAsync(async context =>
        {
            using HttpClient http = new HttpClient { BaseAddress = context.MockServerUri };
            CoreClient client = new CoreClient(http);

            UserDTO? user = await client.GetUserByIdTestAsync(1);

            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
        });
    }
}
