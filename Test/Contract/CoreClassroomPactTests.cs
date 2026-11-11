using System.Net;
using PactNet;
using PactNet.Matchers;

namespace Test.Contract;

public class CoreClassroomPactTests
{
    private readonly IPactBuilderV4 _pact;

    public CoreClassroomPactTests()
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
    public async Task GetAllClassroomsAsync_satisfies_the_contract_with_core()
    {
        // IMPORTANT: This may look like it needs to reflect what is actually returned by core,
        // but this is how the consumer drives the contract! This mocks a response by core, but is solely defined by foodplanner-api.
        // When changes are made to foodplanner-api endpoints which have a Pact test, this test should fail (which is expected and very important that it does)
        // since a change to an endpoint, means a change to a DTO, which is used by these tests. Assuming the field added or removed from the DTO is required, else it will not fail!
        _pact
            .UponReceiving("a request for all classes") // is the description of the interaction
            .WithRequest(HttpMethod.Get, "/legacy/foodplanner/api/Classrooms/GetAll")
            .WillRespond() // indicator that the next information is the response
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(Match.MinType(new
            {
                classId = Match.Integer(1), 
                className = Match.Regex("1.Y", @"[0-9]\.[A-Z]"), // matches class names of the form "1.A", "2.B", "3.Y" etc.
            }, 1));

        // VerifyAsync starts the mock server, runs the code below against it, and stops it again.
        // When the test passes, what is written to the contracts directory is the
        // ideal representation of the response object which is required from core-v2.
        await _pact.VerifyAsync(async context =>
        {
            // instantiating an http client where the base address, typically localhost or the ip addr. of an actual server, is the Pact mock server
            var client = new CoreClient(new HttpClient { BaseAddress = context.MockServerUri }); // context.MockServerUri is the mock server provided by Pact

            var classrooms = (await client.GetAllClassroomsAsync()).ToList();

            Assert.Equal(1, classrooms[0].ClassId);
            Assert.Equal("1.Y", classrooms[0].ClassName);
        });
    }
}
