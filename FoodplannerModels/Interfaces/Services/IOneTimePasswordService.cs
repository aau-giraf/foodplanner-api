namespace FoodplannerModels.Codes
{
    public interface IOneTimePasswordService
    {
        Task<int> CreateOneTimePassword(int userID, int? childUser);
        Task<int> RedeemOneTimePassword(string code, int usedByUser);
        Task<string> GenerateUniqueSixDigitCodeAsync();
    }
}
