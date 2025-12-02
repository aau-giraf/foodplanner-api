namespace FoodplannerModels.Codes
{
    public interface IOneTimePasswordService
    {
         Task<int> CreateOneTimePassword(int userID, int? childUser);
        Task<OneTimePassword> GetOneTimePassword(string code);
        Task<int> UpdateOneTimePassword(OneTimePassword OTP);
        Task<bool> CheckIfCodeAlreadyExists(string code);
        
        Task<int> RedeemOneTimePassword(string code);
    }
}
