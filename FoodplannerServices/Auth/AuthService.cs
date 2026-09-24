using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodplannerModels.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using FoodplannerServices.Secret;
using FoodplannerModels.Auth;

// Adds the AuthService to the FoodplannerService.Auth namespace 
namespace FoodplannerServices.Auth
{
    public class AuthService : IAuthService
    {
        // Read only fields
        private readonly IConfiguration _configuration;
        private readonly ISecretLoader _secretsLoader;

        // Constructor
      public AuthService(IConfiguration configuration, ISecretLoader secretsLoader)
        {
            _configuration = configuration;
            _secretsLoader = secretsLoader;
        }

        // Generates a JWT for a user
        public string GenerateJWTToken(User user)
        {
            // Converts user information into claims
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("RoleApproved", user.RoleApproved.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Converts user role into a claim
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (user.Role.HasFlag(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }
            }

            // Converts secret to bytes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretsLoader.GetSecret("JWT_SECRET")));

            // Combines HmacSha256 and key to a signing credential
            // HmacSha256 = cryptographic algorithm 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // Configurates the token
            var token = new JwtSecurityToken(
                issuer: _configuration["ApplicationSettings:JWT_Issuer"],
                audience: _configuration["ApplicationSettings:JWT_Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["ApplicationSettings:JwtExpireDays"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Retrieves ID from JWT with the "Bearer " prefix
        public string RetrieveIdFromJwtToken(string token)
        {
            var jwtToken = ParseToken(token);
            // Retrieve the Id claim
            var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            // Return the Id claim value or a message if not found
            return idClaim != null ? idClaim.Value : "Id claim not found.";
        }

        // Retrieves ID from JWT without the "Bearer " prefix
        public string RetrieveIdFromJwtTokenNoBearer(string token)
        {
            var jwtToken = ParseTokenNoBearer(token);
            // Retrieve the Id claim
            var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            // Return the Id claim value or a message if not found
            return idClaim != null ? idClaim.Value : "Id claim not found.";
        }

        // Retrieves Role by JWT
        public string RetrieveRoleFromJwtToken(string token)
        {
            var jwtToken = ParseToken(token);
            // Retrieve the Id claim
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            // Return the Id claim value or a message if not found
            return roleClaim != null ? roleClaim.Value : "Role claim not found.";
        }

        // Parses the token with "Bearer " prefix
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


        // Parses the token without the "Bearer " prefix
        private JwtSecurityToken ParseTokenNoBearer(string token)
        {
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