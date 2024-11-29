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

            _issuer = configuration["ApplicationSettings:JWT_Issuer"];
            _audience = configuration["ApplicationSettings:JWT_Audience"];
            _secret = configuration["ApplicationSettings:JWT_Secret"];

            if (string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_audience) || string.IsNullOrEmpty(_secret))
            {
                throw new InvalidOperationException("JWT settings must be defined in appsettings.json.");
            }
        }

        // Utility method to generate a test JWT token to re-use throughout testing.
        private string GenerateJwtToken(Guid userId, string role, bool roleApproved, DateTime? expiration = null)
        {
            expiration ??= DateTime.UtcNow.AddDays(30); // Default expiration in 30 days if not provided, matches our settings in program.

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
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("Parent")]
        [InlineData("Teacher")]
        [InlineData("Child")]
        public void GenerateJwtToken_ShouldIncludeExpectedClaims(string role)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleApproved = true;
            var token = GenerateJwtToken(userId, role, roleApproved);

            // Act
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert
            Assert.NotNull(token);
            Assert.Equal(userId.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(role, jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            Assert.Equal(roleApproved.ToString(), jwtToken.Claims.First(c => c.Type == "RoleApproved").Value);
        }

        [Fact]
        public void ValidateJwtToken_ShouldPass_ForValidToken()
        {
            // Arrange
            var token = GenerateJwtToken(Guid.NewGuid(), "Admin", true);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret))
            };

            var handler = new JwtSecurityTokenHandler();

            // Act & Assert
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            Assert.NotNull(principal);
            Assert.NotNull(validatedToken);
        }

        [Fact]
        public void GenerateJwtToken_ShouldSetCorrectExpiration()
        {
            // Arrange
            var token = GenerateJwtToken(Guid.NewGuid(), "Admin", true);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Act
            var expiry = jwtToken.ValidTo;

            // Assert
            Assert.True(expiry > DateTime.UtcNow);
            Assert.True(expiry <= DateTime.UtcNow.AddDays(30));
        }

        [Fact]
        public void GenerateJwtToken_ShouldFailValidation_IfRoleNotApproved()
        {
            // Arrange
            var token = GenerateJwtToken(Guid.NewGuid(), "Admin", false);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret))
            };

            var handler = new JwtSecurityTokenHandler();

            // Act & Assert
            var exception = Assert.Throws<SecurityTokenValidationException>(() =>
            {
                var principal = handler.ValidateToken(token, validationParameters, out _);
                
                var roleApprovedClaim = principal.Claims.FirstOrDefault(c => c.Type == "RoleApproved")?.Value;
                if (roleApprovedClaim != true.ToString())
                {
                    throw new SecurityTokenValidationException("Inactive user status");
                }
            });

            Assert.Contains("Inactive user status", exception.Message);
        }

        [Fact]
        public void ValidateJwtToken_ShouldHandleClockSkew()
        {
            var token = GenerateJwtToken(Guid.NewGuid(), "Admin", true);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var handler = new JwtSecurityTokenHandler();

            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            Assert.NotNull(principal);
            Assert.NotNull(validatedToken);
        }

        [Fact]
        public void ValidateJwtToken_ShouldFail_WhenTokenIsExpired()
        {
            var expiredToken = GenerateJwtToken(Guid.NewGuid(), "Admin", true, DateTime.UtcNow.AddMinutes(-1));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true, 
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
                ClockSkew = TimeSpan.Zero 
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // Act & Assert
            var exception = Assert.Throws<SecurityTokenExpiredException>(() =>
            {
                tokenHandler.ValidateToken(expiredToken, tokenValidationParameters, out _);
            });

            // Assert that the exception message contains "Lifetime validation failed"
            Assert.Contains("Lifetime validation failed", exception.Message);
        }

        [Fact]
        public void ValidateToken_ShouldPass_WhenValidTokenIsProvided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = "Admin";
            var roleApproved = true;
            var token = GenerateJwtToken(userId, role, roleApproved);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,  
                ValidateAudience = true,
                ValidAudience = _audience,  
                ValidateLifetime = true,  
                ValidateIssuerSigningKey = true,  
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
                ClockSkew = TimeSpan.Zero  
            };

            var handler = new JwtSecurityTokenHandler();

            // Act
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            // Assert
            Assert.NotNull(principal);  
            Assert.NotNull(validatedToken);  
            Assert.Equal(userId.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);  
            Assert.Equal(role, principal.FindFirst(ClaimTypes.Role)?.Value);  
            Assert.Equal(roleApproved.ToString(), principal.FindFirst("RoleApproved")?.Value);  
        }
    }
}
