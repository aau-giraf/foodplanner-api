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

    public async Task<int> CreateOneTimePassword(int userID, int? childUser)
    {
        if(childUser != null)
        {
            var child = await _childrenRepository.GetChildByIdAsync(childUser.Value);
            if(child == null)
                throw new Exception("Id given is not for a child");
        }
        var otp = new OneTimePassword
        {
            GeneratedBy = userID,
            CreatedOn = DateTime.Now,
            ExpiresOn = DateTime.Now.AddDays(1),
            Used = false,
            UsedByUser = null,
            ChildUser = childUser,
            Code = await GenerateUniqueSixDigitCodeAsync()
        };
        var status = await _oneTimePasswordRepository.InsertAsync(otp);
        if (status == 0)
            throw new Exception("One Time Password failed creation");

        return int.Parse(otp.Code);
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
    
    // Child is being added to parent
    if (otp.ChildUser == null)
    {
        await _oneTimePasswordRepository.DeleteAsync(code);
        return await _childrenRepository.AddParentToChildAsync(
            otp.GeneratedBy,
            otp.UsedByUser.Value
        );
    }
    // Parent is being added to child
    else
    {
        await _oneTimePasswordRepository.DeleteAsync(code);
        return await _childrenRepository.AddParentToChildAsync(
            otp.UsedByUser.Value,
            otp.ChildUser.Value
        );
    }
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
