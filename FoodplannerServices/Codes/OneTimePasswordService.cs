using FoodplannerModels.Account;
using FoodplannerModels.Codes;

namespace FoodplannerServices.Codes;

public class OneTimePasswordService(IOneTimePasswordRepository oneTimePasswordRepository, IChildrenRepository childrenRepository) : IOneTimePasswordService
{
    // Creates a one time password (otp)
    // The opt expires after 24 hours
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
        var status = await oneTimePasswordRepository.InsertAsync(otp);
        if (status == 0)
            throw new Exception("One Time Password failed creation");
        return int.Parse(otp.Code);
    }

    // Checks and records who redeems the otp 
    public async Task<int> RedeemOneTimePassword(string code, int usedByUser)
    {
        // Checks if otp has expired
        if (await oneTimePasswordRepository.CheckIfCodeExpiredAsync(code))
            return 0;

        // Retrieves otp from repository
        var otp = await oneTimePasswordRepository.GetFromCodeAsync(code);
        if (otp == null)
            return 0;

        // Links the otp to the user
        otp.UsedByUser = usedByUser;
        await oneTimePasswordRepository.UpdateAsync(otp);

        // Child is being added to parent
        if (otp.ChildUser == null)
        {
            // Deletes the otp from the repository
            await oneTimePasswordRepository.DeleteAsync(code);

            // Adds a relation between child and parent
            return await childrenRepository.AddUserToChildAsync(
                otp.GeneratedBy,
                otp.UsedByUser.Value
            );
        }
        // Parent is being added to child
        else
        {   
            // Deletes the otp from the repository
            await oneTimePasswordRepository.DeleteAsync(code);

            // Adds a relation between parent and child
            return await childrenRepository.AddUserToChildAsync(
                otp.UsedByUser.Value,
                otp.ChildUser.Value
            );
        }
    }

    // Generates a unique 6 digit otp
    public async Task<string> GenerateUniqueSixDigitCodeAsync()
    {
        int code = Random.Shared.Next(100000, 1000000);
        var codes = await oneTimePasswordRepository.GetListOfCodes();

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