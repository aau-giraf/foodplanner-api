using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Test.JwtTest
{
    public class JwtTokenGenerationTests
    {
        private readonly string _issuer = "Aalborg University";
        private readonly string _audience = "Egebakken";
        private readonly string _secret = "Foodplanner_secret_giraf_multi_projcet_102930572926483932629376594y629";

        [Fact]
        public void GenerateJwtToken_ShouldIncludeExpectedClaims()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = "Admin";
            var roleApproved = true;
            var token = GenerateTestToken(userId, role, roleApproved);

            // Act
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert
            Assert.NotNull(token);
            Assert.Equal(userId.ToString(), jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal(role, jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            Assert.Equal(roleApproved.ToString(), jwtToken.Claims.First(c => c.Type == "RoleApproved").Value);
        }

        private string GenerateTestToken(Guid userId, string role, bool roleApproved)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("RoleApproved", roleApproved.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}