using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodplannerModels.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using FoodplannerServices.Secret;
using FoodplannerModels.Auth;

namespace FoodplannerServices.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
      private readonly ISecretLoader _secretsLoader;

      public AuthService(IConfiguration configuration, ISecretLoader secretsLoader)
        {
            _configuration = configuration;
            _secretsLoader = secretsLoader;
        }

        public string GenerateJWTToken(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("RoleApproved", user.RoleApproved.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (user.Role.HasFlag(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretsLoader.GetSecret("JWT_SECRET")));
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