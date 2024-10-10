using AutoMapper;
using FoodplannerModels.Account;

namespace FoodplannerServices.Account;

public class UserService : IUserService {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;


    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync(){

        var user = await _userRepository.GetAllAsync();
        var userDTO = _mapper.Map<IEnumerable<UserDTO>>(user);
        return userDTO;
    }

    public async Task<User> GetUserByIdAsync(int id){
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

    public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        return await _userRepository.GetByEmailAndPasswordAsync(email, password);
    }
}



