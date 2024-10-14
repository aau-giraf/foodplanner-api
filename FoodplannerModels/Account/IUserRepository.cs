using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<int> InsertAsync(User entity);
        Task<int> UpdateAsync(User entity);
        Task<int> DeleteAsync(int id);
        Task<User?> GetPasswordByEmailAsync(string email);
    }
}
