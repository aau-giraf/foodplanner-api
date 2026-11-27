using System.Net.Http.Headers;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerServices.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Test.E2E;

public static class TestAuthentication
{
    public static HttpClient CreateAuthenticatedClient(E2EWebApplicationFactory factory, int userId, UserRole role = UserRole.Parent)
    {
        // Retrieve the authentication service from the API
        var authService = factory.Services.GetRequiredService<IAuthService>();

        //Test user info
        string firstName = "Tester";
        string lastName = "Testington";
        string email = "test@tester.com";
        string password = "1234";

        // Create a test user
        var user = new User
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password,
            Role = role,
            RoleApproved = true
        };

        // Generate a JWT using the existing authentication service
        var token = authService.GenerateJWTToken(user);

        // Create an HTTP client
        var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });

        // Add the JWT to the authorization header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}