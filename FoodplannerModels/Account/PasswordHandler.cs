namespace FoodplannerModels.Account;

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