using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<int> CreateUserAsync(UserDTO user);
        Task<int> UpdateUserAsync(User user);
        Task<int> DeleteUserAsync(int id);
        Task<string?> GetJWTByEmailAndPasswordAsync(string email, string password);
    }
}
