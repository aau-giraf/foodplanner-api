using foodplanner_api.Data.Repositories;
using foodplanner_api.Models;

namespace foodplanner_api.Service;


public class UserService {
    private readonly IRepository<User> _userRepository;


    public UserService(IRepository<User> userRepository){
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(){
        return await _userRepository.GetAllAsync();
    }

    public async Task<User> GetAllUsersByIdAsync(int id){
        return await _userRepository.GetByIdAsync(id);
    }
    
    public async Task<int> CreateUserAsync(User user){
        return await _userRepository.InsertAsync(user);
    }
    
    public async Task<int> UpdateUserAsync(User user){
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<int> DeleteUserAsync(int id){
        return await _userRepository.DeleteAsync(id);
    }
}



