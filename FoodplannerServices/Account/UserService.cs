using AutoMapper;
using FoodplannerApi.Helpers;
using FoodplannerModels.Account;

namespace FoodplannerServices.Account;

public class UserService : IUserService {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly AuthService _authService;


    public UserService(IUserRepository userRepository, IMapper mapper, AuthService authService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _authService = authService;
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
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Status = "inactive";
        return await _userRepository.InsertAsync(user);
    }
    
    public async Task<int> UpdateUserAsync(User user){
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<int> DeleteUserAsync(int id){
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<string?> GetJWTByEmailAndPasswordAsync(string email, string password)
    {
        var user = await _userRepository.GetPasswordByEmailAsync(email);
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user?.Password);

        if (!isPasswordValid)
        {
            return null;
        }
        var jwt = _authService.GenerateJWTToken(user);
        return jwt;
    }
}



