using System.Net;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // assigning consumer, provider and where to place the pact to the _pact object
        _pact = Pact.V4(
            "foodplanner-api", // consumer name, goes in the file
            "core",            // provider name, goes in the file
            config).WithHttpInteractions();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateUserById_satisfies_the_contract_with_core()
    {
        _pact
            // The provider state tells core to seed the user before verifying.
            .UponReceiving("a request for updating a user") // is the description of the interaction
            .Given("the user belonging to the id exists")
            .WithRequest(HttpMethod.Put, "/legacy/foodplanner/api/admin/UpdateUser/1")
            .WithJsonBody(new
            {
                firstName = "string",
                lastName = "string",
                email = "user@example.com",
                password = "string"
            })
            .WillRespond() // indicator that the next information is the response
            .WithStatus(HttpStatusCode.NoContent);

        // VerifyAsync starts the mock server, runs the code below against it, and stops it again.
        // When the test passes, what is written to the contracts directory is the
        // ideal representation of the response object which is required from core-v2.
        await _pact.VerifyAsync(async context =>
        {
            // instantiating an http client where the base address, typically localhost or the ip addr. of an actual server, is the Pact mock server
            using var http = new HttpClient { BaseAddress = context.MockServerUri }; // context.MockServerUri is the mock server provided by Pact
            var client = new CoreClient(http);

            var updateResponse = await client.UpdateUserById();

            Assert.Equal(5, 2);
        });
    }
}
