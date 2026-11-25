using System.Net;
using FoodplannerModels.Account;
using PactNet;
using PactNet.Matchers;

namespace Test.Contract;

public class CoreUserPactTests
{
    private readonly IPactBuilderV4 _pact;

    public CoreUserPactTests()
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
    public async Task GetCurrentUser_satisfies_the_contract_with_core()
    {
        _pact
            .UponReceiving("a request for the current user")
            .Given("a user is logged in")
            .WithRequest(HttpMethod.Get, "/legacy/foodplanner/api/Users/GetLoggedIn")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            { // these fields have to be lower case because of some JSON (de)serialization weirdness
                id = Match.Integer(1), 
                firstName = Match.Regex("Hans", @"[a-zA-Z]+"),
                lastName = Match.Regex("Hansen", @"[a-zA-Z]+"),
                email = Match.Regex("hans.hansen@mail.com", @"[a-zA-Z\.\-_]+@[a-zA-Z]+\.[a-z]+"),
                role = Match.Integer(1),
                roleApproved = Match.Type(true),
                archived = Match.Type(true)
            });


        await _pact.VerifyAsync(async context =>
        {
            using HttpClient http = new HttpClient { BaseAddress = context.MockServerUri };
            CoreClient client = new CoreClient(http);

            UserDTO user = await client.GetLoggedInAsync();

            Assert.NotNull(user);
            Assert.Equal(1, user!.Id);
            Assert.Equal("Hans", user.FirstName);
            Assert.Equal("Hansen", user.LastName);
            Assert.Equal("hans.hansen@mail.com", user.Email);
            Assert.Equal(UserRole.Admin, user.Role);
            Assert.True(user.RoleApproved);
            Assert.True(user.Archived);
        });
    }
}