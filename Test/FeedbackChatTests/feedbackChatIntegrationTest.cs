using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FoodplannerModels.FeedbackChat;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
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

        // Assert
        // I repeat, clean the message table before running this test
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new NpgsqlCommand("SELECT COUNT(*) FROM message WHERE content = @Content", connection);
            command.Parameters.AddWithValue("Content", "Hello, world!");
            var count = (long)await command.ExecuteScalarAsync();
            Assert.Equal(1, count);  // Verify that exactly one record was added
        }
    }

    
    [Fact]
    public async Task AddMessageAsync_ReturnsBadRequest_WhenMessageIsEmpty()
    {
        // Arrange
        var messageDto = new AddMessageDTO
        {
            Content = string.Empty, 
            ChatThreadId = 1
        };

        var token = GenerateJwtToken(1, "Parent", true);
        var content = new StringContent(JsonSerializer.Serialize(messageDto), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync("/api/FeedbackChat/AddMessage", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task AddMessageAsync_ReturnsBadRequest_WhenMessageExceedsCharacterLimit()
    {
        // Arrange: Create a message exceeding 1000 characters
        var messageDto = new AddMessageDTO
        {
            Content = new string('A', 1001), // 1001 characters
            ChatThreadId = 1
        };

        var token = GenerateJwtToken(1, "Parent", true);
        var content = new StringContent(JsonSerializer.Serialize(messageDto), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act: Send the request
        var response = await _client.PostAsync("/api/FeedbackChat/AddMessage", content);

        // Assert: Expect a BadRequest response
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetMessages_ReturnsOk_WhenMessagesExist()
    {
        // Arrange
        var messageDto = new AddMessageDTO
        {
            Content = "Hello, world!",
            ChatThreadId = 2
        };
        
        var token = GenerateJwtToken(1, "Parent", true);
        var content = new StringContent(JsonSerializer.Serialize(messageDto), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act
        var response = await _client.GetAsync("/api/FeedbackChat/GetMessages/2");
        
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
public async Task ArchiveMessage_ReturnsOK_WhenMessageArchived()
{
    // Arrange
    var messageDto = new AddMessageDTO
    {
        Content = "This message will be archived",
        ChatThreadId = 1
    };
    var token = GenerateJwtToken(1, "Parent", true);
    var content = new StringContent(JsonSerializer.Serialize(messageDto), Encoding.UTF8, "application/json");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
    // Create message
    var addMessageResponse = await _client.PostAsync("/api/FeedbackChat/AddMessage", content);
    addMessageResponse.EnsureSuccessStatusCode(); // Ensure message creation was successful

    // Act: Archive the message (using DELETE method)
    var archiveResponse = await _client.DeleteAsync($"/api/FeedbackChat/ArchiveMessage/41");
    
    // Assert: Check if the archive response is OK
    Assert.Equal(HttpStatusCode.OK, archiveResponse.StatusCode);

    // Verify the message is archived by checking the database
    bool isArchived = await CheckIfMessageIsArchivedInDatabase("This message will be archived");

    // Assert: Ensure the archived flag is true (it will return true if archived in the DB)
    Assert.True(isArchived, "The message should be archived in the database.");
}

// Helper method to check the database directly
private async Task<bool> CheckIfMessageIsArchivedInDatabase(string messageContent)
{
    var query = "SELECT archived FROM message WHERE content = @Content";

    using (var connection = new NpgsqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@Content", messageContent);

        var result = await command.ExecuteScalarAsync();

        // Ensure result is not null, and safely cast the result to boolean
        if (result == DBNull.Value || result == null)
        {
            return false; // Return false if no value found or if result is DBNull
        }

        return (bool)result; // Return true if archived field is true
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
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
