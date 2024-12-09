using FoodplannerModels.Account;

namespace FoodplannerModels.Auth;

public interface IAuthService
{
    string GenerateJWTToken(User user);
    string RetrieveIdFromJwtToken(string token);
    string RetrieveRoleFromJwtToken(string token);
}