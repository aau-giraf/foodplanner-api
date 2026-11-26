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
    [Trait("Category", "Admin")]
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
                id = Match.Integer(1),
                firstName = Match.Regex("Hans", "^[A-Za-z]+$"),
                lastName = Match.Regex("Hansen", "^[A-Za-z]+$"),
                email = Match.Regex("hans.hansen@email.com", "^[A-Za-z0-9._+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$"),
                role = Match.Integer((int)UserRole.Parent),
                roleApproved = Match.Type(true),
                archived = Match.Type(false)
            });

        await _pact.VerifyAsync(async context =>
        {
            using HttpClient http = new HttpClient { BaseAddress = context.MockServerUri };
            CoreClient client = new CoreClient(http);

            UserDTO? user = await client.GetUserByIdTestAsync(1);

            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.Equal("Hans", user.FirstName);
            Assert.Equal("Hansen", user.LastName);
            Assert.Equal("hans.hansen@email.com", user.Email);
            Assert.Equal(UserRole.Parent, user.Role);
            Assert.True(user.RoleApproved);
            Assert.False(user.Archived);
        });
    }
}
