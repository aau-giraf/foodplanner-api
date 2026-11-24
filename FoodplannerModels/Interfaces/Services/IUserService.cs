namespace FoodplannerModels.Account
{
        // Interface for user service 
        public interface IUserService
        {
                Task<IEnumerable<UserDTO>> GetAllUsersAsync();
                Task<UserDTO?> GetUserByIdAsync(int id);
                Task<int> CreateUserAsync(UserCreateDTO userCreate);
                Task<int> CreateChildrenUserAsync(UserCreateChildDTO userCreateChildDto, int parentId);
                Task<int> UpdateUserAsync(UserUpdateDTO user, int id);
                Task<int> DeleteUserAsync(int id);
                Task<UserCredsDTO?> GetJWTByEmailAndPasswordAsync(string email, string password);
                Task<string> UpdateUserPinCodeAsync(string pinCode, int id);
                Task<UserCredsDTO> GetUserByIdAndPinCodeAsync(int id, string pinCode);
                Task<bool> UserHasPinCodeAsync(int id);
                Task<IEnumerable<UserDTO>> GetUsersNotApprovedAsync();
                Task<bool> UserUpdateArchivedAsync(int id);
                Task<bool> UserUpdateRoleApprovedAsync(int id, bool roleApproved);
                Task<IEnumerable<User?>> UserSelectAllNotArchivedAsync();
                Task<UserDTO> GetLoggedInUserAsync(int id);
                Task<int> UpdateUserLoggedInAsync(int id, UserUpdateLoggedInDTO userUpdateLoggedInDto);
                Task<int> UpdateUserPasswordAsync(string password, int id);
        }
}
