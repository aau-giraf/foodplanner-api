namespace FoodplannerModels.Account;

public interface IPasswordHandler
{
    string EncryptPassword(string password);
    bool VerifyPassword(string password, string encodedPassword);
}