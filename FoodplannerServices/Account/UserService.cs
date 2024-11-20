using AutoMapper;
using FoodplannerApi.Helpers;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Identity;

namespace FoodplannerServices.Account;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IChildrenRepository _childrenRepository;
    private readonly IMapper _mapper;
    private readonly AuthService _authService;


    public UserService(IUserRepository userRepository, IMapper mapper, AuthService authService, IChildrenRepository childrenRepository)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _authService = authService;
        _childrenRepository = childrenRepository;
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {

        var user = await _userRepository.GetAllAsync();
        var userDTO = _mapper.Map<IEnumerable<UserDTO>>(user);
        return userDTO;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }


    public async Task<int> CreateUserAsync(UserCreateDTO userCreateDTO)
    {
        var user = _mapper.Map<User>(userCreateDTO);
        if (await _userRepository.EmailExistsAsync(user.Email.ToString()))
        {
            throw new InvalidOperationException("Email eksisterer allerede");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.RoleApproved = false;
        return await _userRepository.InsertAsync(user);

    }

    public async Task<int> UpdateUserAsync(User user)
    {
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<int> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    public async Task<UserCredsDTO?> GetJWTByEmailAndPasswordAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        //bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user?.Password);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            throw new InvalidOperationException("Forkert brugernavn eller adgangskode");
        }

        user.Role = user.Role == "Parent" ? "Child" : user.Role;

        var jwt = _authService.GenerateJWTToken(user);
        var userCreds = new UserCredsDTO
        {
            JWT = jwt,
            Role = user.Role,
            RoleApproved = user.RoleApproved
        };

        return userCreds;
    }

    public async Task<string> UpdateUserPinCodeAsync(string pinCode, int id)
    {
        if (pinCode.ToString().Length != 4)
        {
            throw new InvalidOperationException("Pinkode skal være 4 cifre");
        }
        pinCode = BCrypt.Net.BCrypt.HashPassword(pinCode);
        var pincode = await _userRepository.UpdatePinCodeAsync(pinCode, id);

        return pincode;
    }

    public async Task<UserCredsDTO> GetUserByIdAndPinCodeAsync(int id, string pinCode)
    {
        var pincode = await _userRepository.GetPinCodeByIdAsync(id);
        if (pincode == null)
        {
            throw new InvalidOperationException("Bruger har ikke en pinkode");
        }
        else if (!BCrypt.Net.BCrypt.Verify(pinCode, pincode))
        {
            throw new InvalidOperationException("Forkert pinkode");
        }
        var user = await _userRepository.GetByIdAsync(id);
        user.Id = id;
        if (user == null)
        {
            throw new InvalidOperationException("Bruger ikke fundet");
        }

        var jwt = _authService.GenerateJWTToken(user);
        var userCreds = new UserCredsDTO
        {
            JWT = jwt,
            Role = user.Role,
            RoleApproved = user.RoleApproved
        };

        return userCreds;
    }

    public async Task<bool> UserHasPinCodeAsync(int id)
    {
        return await _userRepository.HasPinCodeAsync(id);
    }

    public async Task<IEnumerable<UserDTO>> GetUsersNotApprovedAsync()
    {
        return await _userRepository.GetAllNotApprovedAsync();
    }

    public async Task<bool> UserUpdateArchivedAsync(int id)
    {
        return await _userRepository.UpdateArchivedAsync(id);
    }

    public async Task<bool> UserUpdateRoleApprovedAsync(int id, bool roleApproved)
    {
        return await _userRepository.UpdateRoleApprovedAsync(id, roleApproved);
    }

    public async Task<IEnumerable<User?>> UserSelectAllNotArchivedAsync()
    {
        return await _userRepository.SelectAllNotArchivedAsync();
    }

    public async Task<UserDTO> GetLoggedInUserAsync(int id)
    {
        var user = await _userRepository.GetLoggedInAsync(id);
        return user;
    }
}



