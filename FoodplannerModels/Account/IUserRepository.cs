using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync(); /*
        Task<int> InsertAsync(User entity);     //
        Task<int> UpdateAsync(User entity);     //
        Task<int> DeleteAsync(int id);          *///
        Task<User?> GetByUserIdAsync(int id);
        Task<IEnumerable<UserDTO>> GetAllNotApprovedAsync();
        Task<string> GetPinCodeByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<string> UpdatePinCodeAsync(string pinCode, int id);
        Task<bool> HasPinCodeAsync(int id);
        Task<bool> UpdateArchivedAsync(int id);
        Task<bool> UpdateRoleApprovedAsync(int id, bool roleApproved);
        Task<IEnumerable<User?>> SelectAllNotArchivedAsync();
        Task<UserDTO> GetLoggedInAsync(int id);
        Task<int> UpdateLoggedInAsync(int id, UserUpdateDTO userUpdate);
        Task<int> UpdatePasswordAsync(string password, int id);
    }
}
