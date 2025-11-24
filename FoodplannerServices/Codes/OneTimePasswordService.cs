using AutoMapper;
using FoodplannerModels.Account;
using FoodplannerModels.Codes;

namespace FoodplannerServices.Codes;

public class OneTimePasswordService : IOneTimePasswordService
{
    private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
    private readonly IChildrenService _childrenService;
    public OneTimePasswordService(IOneTimePasswordRepository oneTimePasswordRepository, IChildrenService childrenService) 
    {
        _oneTimePasswordRepository = oneTimePasswordRepository;
        _childrenService = childrenService;
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

    public async Task<OneTimePassword> GetOneTimePassword(string code)
    {
        return await _oneTimePasswordRepository.GetFromCodeAsync(code);
    }

    public async Task<int> RedeemOneTimePassword(string code)
    {
        var OTP = await GetOneTimePassword(code);
        var result = await _childrenService.AddParentToChildAsync(OTP.GeneratedBy, OTP.UsedByUser.Value);
        return result;
    }

    public async Task<int> UpdateOneTimePassword(OneTimePassword OTP)
    {
        return await _oneTimePasswordRepository.UpdateAsync(OTP);
    }
}
