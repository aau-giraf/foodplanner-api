using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Test.JwtTest
{
    public class JwtTokenGenerationTests
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secret;

        public JwtTokenGenerationTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _issuer = configuration["ApplicationSettings:JWT_Issuer"] ?? "DefaultIssuer";
            _audience = configuration["ApplicationSettings:JWT_Audience"] ?? "DefaultAudience";
            _secret = configuration["ApplicationSettings:JWT_Secret"] ?? "DefaultSecret";
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("Parent")]
        [InlineData("Teacher")]
        [InlineData("Child")]
        [InlineData("Guest")]
        public void GenerateJwtToken_ShouldIncludeExpectedClaims(string role)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleApproved = true;
            var token = GenerateTestToken(userId, role, roleApproved);

            // Act
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert
            Assert.NotNull(token);
            Assert.Equal(userId.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(role, jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            Assert.Equal(roleApproved.ToString(), jwtToken.Claims.First(c => c.Type == "RoleApproved").Value);
        }

        private string GenerateTestToken(Guid userId, string role, bool roleApproved)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("RoleApproved", roleApproved.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}