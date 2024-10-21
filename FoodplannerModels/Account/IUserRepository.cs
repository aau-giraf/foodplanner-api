﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<string> GetPinCodeByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<int> InsertAsync(User entity);
        Task<int> UpdateAsync(User entity);
        Task<int> DeleteAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<string> UpdatePinCodeAsync(string pinCode, int id);
    }
}
