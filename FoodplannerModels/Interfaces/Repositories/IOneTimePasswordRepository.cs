namespace FoodplannerModels.Codes;

public interface IOneTimePasswordRepository
{
    Task<int> InsertAsync(OneTimePassword createOTP);
    Task<int> UpdateAsync(OneTimePasswordDTO createOTP);
    Task<int> DeleteAsync(string code);
    Task<OneTimePasswordDTO> GetFromCodeAsync(string code);
    Task<bool> CheckIfCodeExpiredAsync(string code);
    Task<IEnumerable<string>> GetListOfCodes();
}