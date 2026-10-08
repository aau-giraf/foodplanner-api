namespace FoodplannerModels.Account
{
    // Interface for user repository 
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<User>> GetAllNotApprovedAsync();
        Task<string> GetPinCodeByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<string> UpdatePinCodeAsync(string pinCode, int id);
        Task<bool> HasPinCodeAsync(int id);
        Task<bool> UpdateArchivedAsync(int id);
        Task<bool> UpdateRoleApprovedAsync(int id, bool roleApproved);
        Task<IEnumerable<User?>> SelectAllNotArchivedAsync();
        Task<User> GetLoggedInAsync(int id);

        Task<int> UpdateLoggedInAsync(int id, UserUpdateLoggedInDTO userUpdateLoggedInDto);
        Task<int> UpdatePasswordAsync(string password, int id);
    }
}
