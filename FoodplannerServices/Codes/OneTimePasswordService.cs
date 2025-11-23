using AutoMapper;
using FoodplannerModels.Account;
using FoodplannerModels.Codes;

namespace FoodplannerServices.Codes;

public class OneTimePasswordService : IOneTimePasswordService
{
    private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
    public OneTimePasswordService(IOneTimePasswordRepository oneTimePasswordRepository) 
    {
        _oneTimePasswordRepository = oneTimePasswordRepository;
    }

    public async Task<int> CreateOneTimePassword(int userID)
    {
        var OTP = new OneTimePassword();

        OTP.GeneratedBy = userID;
        OTP.Code = "123456";
        OTP.CreatedOn = DateTime.Now;
        OTP.ExpiresOn = OTP.CreatedOn.AddDays(1);
        OTP.Used = false;
        OTP.UsedByUser = null;

        var id = await _oneTimePasswordRepository.InsertAsync(OTP);

        return id;
    }
}
