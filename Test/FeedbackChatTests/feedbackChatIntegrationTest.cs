﻿using System.IdentityModel.Tokens.Jwt;
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
    public async Task GetMessages_ReturnsOk_AndCorrectMessages()
    {
        // Arrange: Add a single message to the database
        var messageDto = new AddMessageDTO
        {
            Content = "Hello, world!",
            ChatThreadId = 1
        };

        var token = GenerateJwtToken(1, "Parent", true);
        var content = new StringContent(JsonSerializer.Serialize(messageDto), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act 1: Send the message to add it to the database
        var addMessageResponse = await _client.PostAsync("/api/FeedbackChat/AddMessage", content);
        addMessageResponse.EnsureSuccessStatusCode(); // Ensure message is added successfully

        // Act 2: Call the GetMessages endpoint
        var getMessagesResponse = await _client.GetAsync("/api/FeedbackChat/GetMessages/1");

        // Assert: Ensure the response status is OK
        Assert.Equal(HttpStatusCode.OK, getMessagesResponse.StatusCode);

        // Act 3: Parse the response content
        var responseContent = await getMessagesResponse.Content.ReadAsStringAsync();

        // Assert: Verify the returned content includes the added message
        Assert.Contains("Hello, world!", responseContent); // Ensure the message appears in the response
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

        // Add the message to the database
        var addMessageResponse = await _client.PostAsync("/api/FeedbackChat/AddMessage", content);
        Assert.Equal(HttpStatusCode.Created, addMessageResponse.StatusCode); // Ensure the message is added

        // Retrieve the ID of the newly added message from the database
        var messageId = await GetMessageIdByContent("This message will be archived");
        Assert.NotNull(messageId); // Ensure the message ID is retrieved

        // Act: Archive the message
        var archiveResponse = await _client.DeleteAsync($"/api/FeedbackChat/ArchiveMessage/{messageId}");
        Assert.Equal(HttpStatusCode.OK, archiveResponse.StatusCode);

        // Verify the message is archived by checking the database
        bool isArchived = await CheckIfMessageIsArchivedInDatabase(messageId);
        Assert.True(isArchived, "The message should be archived in the database.");
    }

    
    
    private async Task<int?> GetMessageIdByContent(string messageContent)
    {
        var query = "SELECT message_id FROM message WHERE content = @Content";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Content", messageContent);

            var result = await command.ExecuteScalarAsync();

            // Ensure result is not null and safely cast to int
            if (result == DBNull.Value || result == null)
            {
                return null; // Return null if no value found
            }
            return (int)result; // Return the ID
        }
    }
    
    [Fact]
    public async Task UpdateMessage_ReturnsOk_WhenMessageEdited()
    {
        // Arrange: Create and add a message to the database
        var messageDto = new AddMessageDTO
        {
            Content = "Original content",
            ChatThreadId = 1
        };

        var token = GenerateJwtToken(1, "Parent", true);
        var content = new StringContent(JsonSerializer.Serialize(messageDto), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var addMessageResponse = await _client.PostAsync("/api/FeedbackChat/AddMessage", content);
        addMessageResponse.EnsureSuccessStatusCode(); // Ensure that message was added successfully

        // Retrieve the message ID from the database (since ID is auto-incremented)
        int messageId = 0;
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new NpgsqlCommand("SELECT message_id FROM message WHERE content = @Content LIMIT 1", connection);
            command.Parameters.AddWithValue("@Content", "Original content");
            var result = await command.ExecuteScalarAsync();
            messageId = (int)result;
        }

        // Create the DTO for updating the message
        var updateMessageDto = new UpdateMessageDTO
        {
            MessageId = messageId,
            Content = "Updated content"
        };

        // Act: Send the request to update the message
        var updateContent = new StringContent(JsonSerializer.Serialize(updateMessageDto), Encoding.UTF8, "application/json");
        var updateResponse = await _client.PutAsync("/api/FeedbackChat/UpdateMessage", updateContent);

        // Assert: Ensure the response is OK
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Verify the message content is updated in the database and is_edited flag is true
        bool isMessageUpdated = false;
        bool isEdited = false;
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new NpgsqlCommand("SELECT content, is_edited FROM message WHERE message_id = @MessageId", connection);
            command.Parameters.AddWithValue("@MessageId", messageId);
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    isMessageUpdated = reader.GetString(0) == "Updated content"; // Check if content was updated
                    isEdited = reader.GetBoolean(1); // Check if is_edited flag is true
                }
            }
        }

        // Assert: Ensure the message content was updated and is_edited is set to true
        Assert.True(isMessageUpdated, "The message content should be updated.");
        Assert.True(isEdited, "The message should have the 'is_edited' flag set to true.");
    }


// Helper method to check the database directly
private async Task<bool> CheckIfMessageIsArchivedInDatabase(int? messageId)
{
    var query = "SELECT archived FROM message WHERE message_id = @messageId";

    using (var connection = new NpgsqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@messageId", messageId);

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
