using FoodplannerModels.Account;

namespace FoodplannerModels.Auth;

public interface IAuthService
{
    public string GenerateJWTToken(User user);
    public string RetrieveIdFromJwtToken(string token);
    public string RetrieveRoleFromJwtToken(string token);
}