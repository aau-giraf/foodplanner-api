namespace FoodplannerModels.Account
{
    // Interface for user repository 
    public interface IUserRepository
    {
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllNotApprovedAsync();
        Task<bool> EmailExistsAsync(string email);
        Task<int> InsertAsync(User entity);
        Task<int> UpdateAsync(User entity);
        Task<string> UpdatePinCodeAsync(string pinCode, int id);
        Task<string> GetPinCodeByIdAsync(int id);
        Task<bool> HasPinCodeAsync(int id);
        Task<bool> UpdateArchivedAsync(int id, bool archived);
        Task<bool> UpdateRoleApprovedAsync(int id, bool roleApproved);
        Task<IEnumerable<User?>> SelectAllNotArchivedAsync();
        Task<User> GetLoggedInAsync(int id);
        Task<int> UpdateLoggedInAsync(int id, UserUpdateLoggedInDTO userUpdateLoggedInDto);
        Task<int> UpdatePasswordAsync(string password, int id);
    }
}