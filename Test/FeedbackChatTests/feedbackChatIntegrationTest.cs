using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FoodplannerModels.FeedbackChat;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Moq;
using FluentAssertions;
using FoodplannerApi.Helpers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Dapper;
using FoodplannerDataAccessSql;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using Microsoft.Extensions.DependencyInjection;
using Test.JwtTest;

public class FeedbackChatIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly PostgreSQLConnectionFactory _connectionFactory;
    
    public FeedbackChatIntegrationTest(WebApplicationFactory<Program> factory)
    {
        var mockAuthService = new Mock<IAuthService>();
        var mockChatrepo = new Mock<IChatRepository>();
        var mockChatService = new Mock<IChatService>();
        var mockChildrenRepo = new Mock<IChildrenRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        

        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(It.IsAny<string>()))
            .Returns("1"); // Simulate extracting userId from token

        mockChatService
            .Setup(chat => chat.AddMessageAsync(It.IsAny<AddMessageDTO>(), It.IsAny<int>()))
            .ReturnsAsync(true); // Simulate successful message creation

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(mockAuthService.Object);
                services.AddSingleton(mockChatService.Object);
                services.AddSingleton(mockChatrepo.Object);
                services.AddSingleton(mockChildrenRepo.Object);
                services.AddSingleton(mockUserRepo.Object);
            });
        }).CreateClient();
    }
    
    private const string Issuer = "TestIssuer";
    private const string Audience = "TestAudience";
    private const string Secret = "TestSecretKey123456789thisissoverysecretyesindeeeeeeeeeeeeeeed";

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
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task AddMessageAsync_ReturnsCreated()
    {
        
        // Arrange
        //make user
       

        var message = new AddMessageDTO
        {
            Content = "heælooo",
            ChatThreadId = 1
        };
        var token = GenerateJwtToken(
            1, "Parent", true);

        var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
        Debug.WriteLine($"Serialized Message: {content}");
        System.Diagnostics.Debug.WriteLine($"This is a debug message. {content}");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync("api/FeedbackChat/AddMessage", content);

        // Assert
        Console.WriteLine(response.Content);
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}