using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Test.JwtTest
{
    public class MissingClaimsException : Exception
    {
        public MissingClaimsException(string message) : base(message) { }
    }

    public class JwtTokenTests
    {
        private const string Issuer = "TestIssuer";
        private const string Audience = "TestAudience";
        private const string Secret = "TestSecretKey123456789thisissoverysecretyesindeeeeeeeeeeeeeeed";

        private string GenerateJwtToken(Guid userId, string role, bool roleApproved, DateTime? expiration = null)
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

            // Act
            var token = GenerateJwtToken(userId, role, roleApproved);
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
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                ClockSkew = TimeSpan.FromMinutes(5)
            };
            var handler = new JwtSecurityTokenHandler();

            // Act
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            // Assert
            Assert.NotNull(principal);
            Assert.NotNull(validatedToken);
        }

        [Fact]
        public void ValidateJwtToken_ShouldFail_WhenTokenIsExpired()
        {
            // Arrange
            var expiredToken = GenerateJwtToken(Guid.NewGuid(), "Admin", true, DateTime.UtcNow.AddMinutes(-1));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                ClockSkew = TimeSpan.Zero
            };
            var handler = new JwtSecurityTokenHandler();

            // Act & Assert
            Assert.Throws<SecurityTokenExpiredException>(() =>
            {
                handler.ValidateToken(expiredToken, validationParameters, out _);
            });
        }
        
        [Fact]
        public void ValidateJwtToken_ShouldFail_WhenRequiredClaimIsMissing()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Role, "Admin") }; // Missing NameIdentifier and RoleApproved

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret))
            };
            var handler = new JwtSecurityTokenHandler();

            // Act & Assert
            Assert.Throws<MissingClaimsException>(() =>
            {
                var principal = handler.ValidateToken(jwtToken, validationParameters, out _);

                var missingClaims = new List<string>();
                if (!principal.Claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
                    missingClaims.Add("NameIdentifier");
                if (!principal.Claims.Any(c => c.Type == "RoleApproved"))
                    missingClaims.Add("RoleApproved");

                if (missingClaims.Any())
                {
                    throw new MissingClaimsException($"Required claims missing: {string.Join(", ", missingClaims)}");
                }
            });
        }

        [Fact]
        public void ValidateJwtToken_ShouldFail_ForInvalidIssuerOrAudience()
        {
            // Arrange
            var token = GenerateJwtToken(Guid.NewGuid(), "Admin", true);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "InvalidIssuer", // Invalid issuer
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret))
            };
            var handler = new JwtSecurityTokenHandler();

            // Act & Assert
            Assert.Throws<SecurityTokenInvalidIssuerException>(() =>
            {
                handler.ValidateToken(token, validationParameters, out _);
            });
        }

        [Fact]
        public void ValidateJwtToken_ShouldFail_ForTamperedToken()
        {
            // Arrange
            var token = GenerateJwtToken(Guid.NewGuid(), "Admin", true);
            var tamperedToken = token.Substring(0, token.Length - 1); // Remove the last character of the token

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret))
            };
            var handler = new JwtSecurityTokenHandler();

            // Act & Assert
            var exception = Assert.ThrowsAny<Exception>(() =>
            {
                handler.ValidateToken(tamperedToken, validationParameters, out _);
            });

            // Check the exception type or message, "signature" ensures it's the signature exception that is tested.
            Assert.Contains("signature", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
