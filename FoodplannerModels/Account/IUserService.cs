﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<IEnumerable<UserDTO>> GetUsersNotApprovedAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<int> CreateUserAsync(UserCreateDTO userCreate);
        Task<int> UpdateUserAsync(User user);
        Task<int> DeleteUserAsync(int id);
        Task<UserCredsDTO?> GetJWTByEmailAndPasswordAsync(string email, string password);
        Task<UserCredsDTO> GetUserByIdAndPinCodeAsync(int id, string pinCode);
        Task<string> UpdateUserPinCodeAsync(string pinCode, int id);
        Task<bool> UserHasPinCodeAsync(int id);
        Task<bool> UserUpdateArchivedAsync(int id);
        Task<bool> UserUpdateRoleApprovedAsync(int id, bool roleApproved);
        Task<IEnumerable<User?>> UserSelectAllNotArchivedAsync();

        Task<UserDTO> GetLoggedInUserAsync(int id);
        Task<int> UpdateUserLoggedInAsync(int id, UserUpdateDTO userUpdate);
        Task<int> UpdateUserPasswordAsync(string password, int id);
    }
}
