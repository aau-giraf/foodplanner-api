using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FoodplannerApi.Helpers
{
    public class AuthService : IAuthService

    {
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJWTToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("RoleApproved", user.RoleApproved.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApplicationSettings:JWT_Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["ApplicationSettings:JWT_Issuer"],
            audience: _configuration["ApplicationSettings:JWT_Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["ApplicationSettings:JwtExpireDays"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string RetrieveIdFromJwtToken(string token)
    {
        var jwtToken = ParseToken(token);
        // Retrieve the Id claim
        var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        // Return the Id claim value or a message if not found
        return idClaim != null ? idClaim.Value : "Id claim not found.";
    }

    public string RetrieveRoleFromJwtToken(string token)
    {
        var jwtToken = ParseToken(token);
        // Retrieve the Id claim
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        // Return the Id claim value or a message if not found
        return roleClaim != null ? roleClaim.Value : "Role claim not found.";
    }

    private JwtSecurityToken ParseToken(string token)
    {
        // Ensure the token starts with "Bearer "
        if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
        {
            throw new ArgumentException("The token must be prefixed with 'Bearer '.");
        }

        // Remove the "Bearer " part from the token
        token = token.Substring(7);

        var handler = new JwtSecurityTokenHandler();

        // Validate if the token is in proper JWT format
        if (!handler.CanReadToken(token))
        {
            throw new ArgumentException("The token is not in a valid JWT format.");
        }

        return handler.ReadJwtToken(token);
    }
    }
}