using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FoodplannerModels.FeedbackChat;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

public class FeedbackChatIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private const string Secret = "TestSecretKeyThatIsExactly32BytesLong123";
    private readonly string _connectionString = "Host=localhost;Port=7654;Username=postgres;Password=postgres;Database=giraf";




    public FeedbackChatIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    //message table has to be dropped before running test, test only expects 1 message
    [Fact]
    public async Task AddMessageAsync_ReturnsCreated_AndWritesToDatabase()
    {
        // Arrange
        var messageDto = new AddMessageDTO
        {
            Content = "Hello, world!",
            ChatThreadId = 1
        };

        var token = GenerateJwtToken(1, "Parent", true);
        var content = new StringContent(JsonSerializer.Serialize(messageDto), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync("/api/FeedbackChat/AddMessage", content);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        // Assert: Check the database
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new NpgsqlCommand("SELECT COUNT(*) FROM message WHERE content = @Content", connection);
            command.Parameters.AddWithValue("Content", "Hello, world!");
            var count = (long)await command.ExecuteScalarAsync();
            Assert.Equal(1, count);  // Verify that exactly one record was added
        }
    }

    private string GenerateJwtToken(int userId, string role, bool roleApproved, DateTime? expiration = null)
    {
        expiration ??= DateTime.UtcNow.AddDays(30);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim("RoleApproved", roleApproved.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TestSecretKeyThatIsExactly32BytesLong123"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "TestIssuer",
            audience: "TestAudience",
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
