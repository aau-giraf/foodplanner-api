namespace FoodplannerModels.Codes
{
    public interface IOneTimePasswordService
    {
         Task<int> CreateOneTimePassword(int userID);
    }
}
