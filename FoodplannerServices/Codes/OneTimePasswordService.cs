using System.Collections.Concurrent;
using FoodplannerModels.Account;
using FoodplannerModels.Codes;
using FoodplannerServices.Codes;

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

    public async Task<int> CreateOneTimePassword(int userID, int? childUser)
    {
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

    public async Task<int> RedeemOneTimePassword(string code, int usedByUser)
    {
        if (await _oneTimePasswordRepository.CheckIfCodeExpiredAsync(code))
            return 0;

        var otp = await _oneTimePasswordRepository.GetFromCodeAsync(code);
        if (otp == null)
            return 0;

        otp.UsedByUser = usedByUser;
        await _oneTimePasswordRepository.UpdateAsync(otp);

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

    public async Task<string> GenerateUniqueSixDigitCodeAsync()
    {
        int code = _random.Next(100000, 1000000);

        var codes = await _oneTimePasswordRepository.GetListOfCodes();

        //Iterate code with linear probing until no collision (if it happens at all)
        while (codes.Contains(code.ToString()))
        {
            code++;
            if (code > 999999)
                code = 100000;
        }

        return code.ToString();
    }
}
