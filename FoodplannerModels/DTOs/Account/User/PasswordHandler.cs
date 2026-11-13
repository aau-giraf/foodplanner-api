
namespace FoodplannerModels.Account;

// Class to contain password encryption and password verification methods
// Methods used in Users service
public class PasswordHandler : IPasswordHandler
{
    
    public string EncryptPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string encodedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, encodedPassword);
    }
}