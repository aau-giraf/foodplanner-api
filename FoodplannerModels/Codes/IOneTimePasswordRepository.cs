using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Codes
{
    public interface IOneTimePasswordRepository
    {
        Task<int> InsertAsync(OneTimePassword createOTP);
        Task<int> UpdateAsync(OneTimePassword createOTP);
        Task<int> DeleteAsync(string code);
        Task<OneTimePassword> GetFromCodeAsync(string code);
        Task<bool> CheckIfCodeExistsAsync(string code);
        Task<bool> CheckIfCodeExpiredAsync(string code);
    }
}
