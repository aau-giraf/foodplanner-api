using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IUserService
    {
        Task<IEnumerable<UserCreateDTO>> GetAllUsersAsync();
        Task<IEnumerable<User?>> GetUsersNotApprovedAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<int> CreateUserAsync(UserCreateDTO userCreate);
        Task<int> UpdateUserAsync(User user);
        Task<int> DeleteUserAsync(int id);
        Task<UserCredsDTO?> GetJWTByEmailAndPasswordAsync(string email, string password);
        Task<string> GetUserByIdAndPinCodeAsync(int id, string pinCode);
        Task<string> UpdateUserPinCodeAsync(string pinCode, int id);
        Task<bool> UserHasPinCodeAsync(int id);
        Task<bool> UserUpdateArchivedAsync(int id, bool archived);
        Task<bool> UserUpdateRoleApprovedAsync(int id, bool roleApproved);
        Task<IEnumerable<User?>> UserSelectAllNotArchivedAsync();        
    }
}
