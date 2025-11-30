using FoodplannerModels.Account;
using FoodplannerServices.Codes;
using FoodplannerModels.Codes;

namespace FoodplannerServices.Codes;

public class OneTimePasswordService : IOneTimePasswordService
{
    private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
    private readonly IChildrenRepository _childrenRepository;
    private readonly Random _random = new();

    public OneTimePasswordService(
        IOneTimePasswordRepository oneTimePasswordRepository,
        IChildrenRepository childrenRepository)
    {
        _oneTimePasswordRepository = oneTimePasswordRepository;
        _childrenRepository = childrenRepository;
    }

    public Task<bool> CheckIfCodeAlreadyExists(string code) =>
        _oneTimePasswordRepository.CheckIfCodeExistsAsync(code);

    public async Task<int> CreateOneTimePassword(int userID)
    {
        var otp = new OneTimePassword
        {
            GeneratedBy = userID,
            CreatedOn = DateTime.Now,
            ExpiresOn = DateTime.Now.AddDays(1),
            Used = false,
            UsedByUser = null,
            Code = await GenerateUniqueSixDigitCodeAsync()
        };

        return await _oneTimePasswordRepository.InsertAsync(otp);
    }

    public Task<OneTimePassword> GetOneTimePassword(string code) =>
        _oneTimePasswordRepository.GetFromCodeAsync(code);

    public async Task<int> RedeemOneTimePassword(string code)
    {
        if (await _oneTimePasswordRepository.CheckIfCodeExpiredAsync(code))
            return 0;

        var otp = await GetOneTimePassword(code);
        if (otp == null)
            return 0;

        if (otp.UsedByUser == null)
            return 0;

        await _oneTimePasswordRepository.DeleteAsync(code);
        return await _childrenRepository.AddParentToChildAsync(
            otp.GeneratedBy,
            otp.UsedByUser.Value
        );
    }

    public async Task<int> UpdateOneTimePassword(OneTimePassword otp)
    {
        if (await CheckIfCodeAlreadyExists(otp.Code))
            return await _oneTimePasswordRepository.UpdateAsync(otp);

        return 0;
    }

    private async Task<string> GenerateUniqueSixDigitCodeAsync()
    {
        int code = _random.Next(100000, 1000000);

        while (await CheckIfCodeAlreadyExists(code.ToString()))
        {
            code++;

            if (code > 999999)
                code = 100000;
        }

        return code.ToString();
    }
}
