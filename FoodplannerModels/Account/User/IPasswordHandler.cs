namespace FoodplannerModels.Account;

// Interface for password handler 
public interface IPasswordHandler
{
    string EncryptPassword(string password);
    bool VerifyPassword(string password, string encodedPassword);
}