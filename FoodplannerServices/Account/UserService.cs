using AutoMapper;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;

// Adds the UserService to the FoodplannerService.Account namespace 
namespace FoodplannerServices.Account;

public class UserService(IUserRepository userRepository, IMapper mapper, IAuthService authService,
        IChildrenRepository childrenRepository, IPasswordHandler passwordHandler) : IUserService
{
    // Retrieves all users from database 
    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        var user = await userRepository.GetAllAsync();
        var userDTO = mapper.Map<IEnumerable<UserDTO>>(user);
        return userDTO;
    }

    // Retrieve user associated with ID
    public async Task<UserDTO?> GetUserByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return mapper.Map<UserDTO?>(user);
    }

    // Creates a new user based on UserCreateDTO (First and Last-name, e-mail, password, and role)
    public async Task<int> CreateUserAsync(UserCreateDTO userCreateDTO)
    {
        var user = mapper.Map<User>(userCreateDTO);

        if (await userRepository.EmailExistsAsync(user.Email))
        {
            throw new InvalidOperationException("Email eksisterer allerede");
        }

        user.Password = passwordHandler.EncryptPassword(user.Password);
        user.RoleApproved = false;

        var id = await userRepository.InsertAsync(user);
        return id;
    }

    // Creates a user account for the child and associates it with a parents user account
    public async Task<int> CreateChildrenUserAsync(UserCreateChildDTO userCreateChildDto, int parentId)
    {
        var user = mapper.Map<User>(userCreateChildDto);
        var parentUser = await userRepository.GetByIdAsync(parentId);
        if (parentUser == null  || (parentUser.Role != UserRole.Parent))
        {
            throw new InvalidOperationException("Forælder ikke fundet eller ugyldig rolle");
        }
        
        if (await userRepository.EmailExistsAsync(user.Email.ToString()))
        {
            throw new InvalidOperationException("Email eksisterer allerede");
        }

        user.Password = passwordHandler.EncryptPassword(user.Password);
        user.RoleApproved = parentUser.RoleApproved;
        user.Role = UserRole.Child;
        var id = await userRepository.InsertAsync(user);

        var child = new Children()
        {
            ChildId = id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ClassId = userCreateChildDto.ClassId
        };
        await childrenRepository.InsertAsync(child);
        await childrenRepository.AddUserToChildAsync(parentId, child.ChildId);

        return id;
    }

    // Updates user
    public async Task<int> UpdateUserAsync(UserUpdateDTO userUpdateDto, int id)
    {
        var user = mapper.Map<User>(userUpdateDto);
        user.Id = id;
        user.Password = passwordHandler.EncryptPassword(user.Password);
        return await userRepository.UpdateAsync(user);
    }

    // Deletes user
    public async Task<int> DeleteUserAsync(int id)
    {
        return await userRepository.DeleteAsync(id);
    }

    // Retrieves JWT by e-mail and password
    public async Task<UserCredsDTO?> GetJWTByEmailAndPasswordAsync(string email, string password)
    {
        var user = await userRepository.GetUserByEmailAsync(email);

        if (user == null || !passwordHandler.VerifyPassword(password, user.Password))
        {
            throw new InvalidOperationException("Forkert brugernavn eller adgangskode");
        }


        var jwt = authService.GenerateJWTToken(user);
        var userCreds = new UserCredsDTO
        {
            JWT = jwt,
            Role = user.Role.ToString(),
            RoleApproved = user.RoleApproved
        };

        return userCreds;
    }

    // Retrieve JWT by Email
    public async Task<UserCredsDTO?> GetJWTByEmailAsync(string email)
    {
        var user = await userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            throw new InvalidOperationException("Forkert email");
        }
        var jwt = authService.GenerateJWTToken(user);
        var userCreds = new UserCredsDTO
        {
            JWT = jwt,
            Role = user.Role.ToString(),
            RoleApproved = user.RoleApproved
        };
        return userCreds;
    }

    // Update user's pincode
    public async Task<string> UpdateUserPinCodeAsync(string pinCode, int id)
    {
        if (pinCode.ToString().Length != 4)
        {
            throw new InvalidOperationException("Pinkode skal være 4 cifre");
        }

        pinCode = passwordHandler.EncryptPassword(pinCode);
        var pincode = await userRepository.UpdatePinCodeAsync(pinCode, id);

        return pincode;
    }

    // Retrieve user ID and their pin code
    public async Task<UserCredsDTO> GetUserByIdAndPinCodeAsync(int id, string pinCode)
    {
        var pincode = await userRepository.GetPinCodeByIdAsync(id);
        if (pincode == null)
        {
            throw new InvalidOperationException("Bruger har ikke en pinkode");
        }
        else if (passwordHandler.VerifyPassword(pinCode, pincode))
        {
            throw new InvalidOperationException("Forkert pinkode");
        }

        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException("Bruger ikke fundet");
        }

        user.Id = id;

        var jwt = authService.GenerateJWTToken(user);
        var userCreds = new UserCredsDTO
        {
            JWT = jwt,
            Role = user.Role.ToString(),
            RoleApproved = user.RoleApproved
        };

        return userCreds;
    }

    // Checks if the user has a pin code
    public async Task<bool> UserHasPinCodeAsync(int id)
    {
        return await userRepository.HasPinCodeAsync(id);
    }

    // Retrieve all not-approved users 
    public async Task<IEnumerable<UserDTO>> GetUsersNotApprovedAsync()
    {
        var users = await userRepository.GetAllNotApprovedAsync();
        return mapper.Map<IEnumerable<UserDTO>>(users);
    }

    // Updates the user's 'archived'-status
    public async Task<bool> UserUpdateArchivedAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException("Bruger ikke fundet");
        }
        var archived = user.Archived; 
        // Flip archived status 
        archived = !archived;
        
        return await userRepository.UpdateArchivedAsync(id, archived);
    }

    // Updates user's role to approved
    public async Task<bool> UserUpdateRoleApprovedAsync(int id, bool roleApproved)
    {
        return await userRepository.UpdateRoleApprovedAsync(id, roleApproved);
    }

    // Selects all not-archived users
    public async Task<IEnumerable<User?>> UserSelectAllNotArchivedAsync()
    {
        return await userRepository.SelectAllNotArchivedAsync();
    }

    // Retrieves the logged-in user by ID
    public async Task<UserDTO> GetLoggedInUserAsync(int id)
    {
        var user = await userRepository.GetLoggedInAsync(id);
        if (user == null) throw new InvalidOperationException("Bruger ikke fundet");
        return mapper.Map<UserDTO>(user);
    }

    // Updates logged-in user information by ID
    public async Task<int> UpdateUserLoggedInAsync(int id, UserUpdateLoggedInDTO userUpdateLoggedInDto)
    {
        var user = await userRepository.UpdateLoggedInAsync(id, userUpdateLoggedInDto);
        return user;
    }

    // Update user's password
    public async Task<int> UpdateUserPasswordAsync(string password, int id)
    {
        password = passwordHandler.EncryptPassword(password);
        var _password = await userRepository.UpdatePasswordAsync(password, id);
        return _password;
    }

    // Checks if user's e-mail already exists
    public async Task<bool> UserEmailExistsAsync(string email)
    {
        var _email = await userRepository.EmailExistsAsync(email);
        return _email;
    }
}