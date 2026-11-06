using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Moq;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Identity;
using Test.Builder;

namespace Test.Service;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IChildrenRepository> _mockChildrenRepository;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IPasswordHandler> _mockPasswordHandler;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockChildrenRepository = new Mock<IChildrenRepository>();
        _mockAuthService = new Mock<IAuthService>();
        _mockPasswordHandler = new Mock<IPasswordHandler>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockAuthService.Object,
            _mockChildrenRepository.Object,
            _mockPasswordHandler.Object
        );
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new User
            {
                Id = 1,
                FirstName = "niels",
                LastName = "nielsen",
                Email = "nielsen@example.com",
                Role = UserRole.Parent,
                Archived = true,
                Password = "",
                RoleApproved = false
            },

            new User
            {
                Id = 2,
                FirstName = "ole",
                LastName = "olsen",
                Email = "olsen@example.com",
                Role = UserRole.Parent,
                Archived = true,
                Password = "",
                RoleApproved = false
            },
        };
        _mockUserRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUsers.Count(), result.Count());
        Assert.All(result, user => Assert.Contains(expectedUsers,
                                                    u => u.Id == user.Id &&
                                                    u.FirstName == user.FirstName &&
                                                    u.LastName == user.LastName &&
                                                    u.Email == user.Email &&
                                                    u.Role == user.Role &&
                                                    u.Archived == user.Archived));
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsAUser_WhenIdIsValid()
    {
        // Arrange
        var expectedId = 1;
        var mail = "nielsen@example.com";
        var userDto = new UserDTO() { Id = expectedId, FirstName = "niels", LastName = "nielsen", Email = mail, Role = UserRole.Teacher, RoleApproved = true };
        var user = new User { Id = expectedId, FirstName = "niels", LastName = "nielsen", Email = mail, Password = "", Role = UserRole.Teacher, RoleApproved = true };

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(userDto.Id))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.Id, result.Id);
        Assert.Equal(userDto.FirstName, result.FirstName);
        Assert.Equal(userDto.LastName, result.LastName);
        Assert.Equal(userDto.Email, result.Email);
        Assert.Equal(userDto.Role, result.Role);
        Assert.Equal(userDto.RoleApproved, result.RoleApproved);
        Assert.Equal(userDto.Archived, result.Archived);
    }


    [Fact]
    public async Task CreateUserAsync_CreatesANewUser()
    {
        // Arrange
        var expectedId = 1;
        var mail = "nielsen@example.com";
        var password = "password";
        var encryptedPassword = "encrypted password";

        var newUser = new UserCreateDTO
        {
            FirstName = "niels",
            LastName = "nielsen",
            Email = mail,
            Password = password,
            Role = "Teacher"
        };

        _mockUserRepository
            .Setup(repo => repo.EmailExistsAsync(mail))
            .ReturnsAsync(false);

        _mockPasswordHandler
            .Setup(handler => handler.EncryptPassword(password))
            .Returns(encryptedPassword);

        _mockUserRepository
            .Setup(repo => repo.InsertAsync(It.IsAny<User>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _userService.CreateUserAsync(newUser);

        // Assert
        Assert.Equal(expectedId, result);

        _mockUserRepository.Verify(repo =>
            repo.InsertAsync(It.Is<User>(user =>
                user.FirstName == newUser.FirstName &&
                user.LastName == newUser.LastName &&
                user.Email == newUser.Email &&
                user.Password == encryptedPassword &&
                user.Role == UserRole.Teacher &&
                user.RoleApproved == false
            )), Times.Once);
    }



    [Fact]
    public async Task CreateUserAsync_ThrowsAnException_WhenEmailAlreadyExists()
    {
        // Arrange
        var mail = "nielsen@example.com";

        var newUser = new UserCreateDTO
        {
            FirstName = "niels",
            LastName = "nielsen",
            Email = mail,
            Password = "password",
            Role = "Teacher"
        };

        _mockUserRepository
            .Setup(repo => repo.EmailExistsAsync(mail))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateUserAsync(newUser));

        _mockUserRepository.Verify(repo =>
            repo.InsertAsync(It.IsAny<User>()), Times.Never);

        _mockPasswordHandler.Verify(handler =>
            handler.EncryptPassword(It.IsAny<string>()), Times.Never);
    }



    [Fact]
    public async Task UpdateUserAsync_UpdatesValueInRepository()
    {
        // Arrange
        var id = 1;
        var password = "newPassword";
        var encryptedPassword = "encryptedPassword";

        var user = new User
        {
            Id = id,
            FirstName = "niels",
            LastName = "nielsen",
            Email = "nielsen@example.com",
            Password = "oldPassword",
            Role = UserRole.Teacher,
            RoleApproved = true
        };

        var userUpdateDto = new UserUpdateDTO
        {
            FirstName = "frederik",
            LastName = "jensen",
            Email = "frederik@example.com",
            Password = password
        };

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(id))
            .ReturnsAsync(user);

        _mockPasswordHandler
            .Setup(handler => handler.EncryptPassword(password))
            .Returns(encryptedPassword);

        _mockUserRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(id);

        // Act
        var result = await _userService.UpdateUserAsync(userUpdateDto, id);

        // Assert
        Assert.Equal(id, result);

        _mockUserRepository.Verify(repo =>
            repo.UpdateAsync(It.Is<User>(updatedUser =>
                updatedUser.Id == id &&
                updatedUser.FirstName == userUpdateDto.FirstName &&
                updatedUser.LastName == userUpdateDto.LastName &&
                updatedUser.Email == userUpdateDto.Email &&
                updatedUser.Password == encryptedPassword &&
                updatedUser.Role == UserRole.Teacher &&
                updatedUser.RoleApproved == true
            )), Times.Once);
    }


    [Fact]
    public async Task DeleteUserAsync_UpdatesValueInRepository()
    {
        // Arrange
        var id = 1;
        var rowsAffected = 1;

        _mockUserRepository
            .Setup(repo => repo.DeleteAsync(id))
            .ReturnsAsync(rowsAffected);

        // Act
        var result = await _userService.DeleteUserAsync(id);

        // Assert
        var expectedRowsChanged = 1;
        Assert.Equal(expectedRowsChanged, result);
    }

    [Fact]
    public async Task GetJWTByEmailAndPasswordAsync_ReturnsUser_WithMatchingCredentials()
    {
        // Arrange
        var expectedJWT = "jwtToken";
        var password = "password";
        var inputUser = new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "encrypted password", Role = UserRole.Teacher, RoleApproved = true };
        var expectedCreds = new UserCredsDTO { JWT = expectedJWT, Role = inputUser.Role.ToString(), RoleApproved = inputUser.RoleApproved };

        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(inputUser.Email))
            .ReturnsAsync(inputUser);
        _mockPasswordHandler
            .Setup(handler => handler.VerifyPassword(password, inputUser.Password))
            .Returns(true);
        _mockAuthService
            .Setup(auth => auth.GenerateJWTToken(inputUser))
            .Returns(expectedJWT);

        // Act
        var result = await _userService.GetJWTByEmailAndPasswordAsync(inputUser.Email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCreds.JWT, result.JWT);
        Assert.Equal(expectedCreds.Role, result.Role);
        Assert.Equal(expectedCreds.RoleApproved, result.RoleApproved);
    }

    [Theory]
    [InlineData(UserRole.Parent)]
    [InlineData(UserRole.Child)]
    public async Task GetJWTByEmailAndPasswordAsync_ReturnsUser_WithChildCredentials(UserRole inputUserRole)
    {
        // Arrange
        var expectedJWT = "jwtToken";
        var password = "password";
        var inputUser = new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "encrypted password", Role = inputUserRole, RoleApproved = true };
        var expectedCreds = new UserCredsDTO { JWT = expectedJWT, Role = inputUserRole.ToString(), RoleApproved = inputUser.RoleApproved };

        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(inputUser.Email))
            .ReturnsAsync(inputUser);
        _mockPasswordHandler
            .Setup(handler => handler.VerifyPassword(password, inputUser.Password))
            .Returns(true);
        _mockAuthService
            .Setup(auth => auth.GenerateJWTToken(inputUser))
            .Returns(expectedJWT);

        // Act
        var result = await _userService.GetJWTByEmailAndPasswordAsync(inputUser.Email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCreds.JWT, result.JWT);
        Assert.Equal(expectedCreds.Role, result.Role);
        Assert.Equal(expectedCreds.RoleApproved, result.RoleApproved);
    }

    [Fact]
    public async Task GetJWTByEmailAndPasswordAsync_ThrowsAnException_WhenGivenWrongEmailOrPassword()
    {
        // Arrange
        var expectedJWT = "jwtToken";
        var inputUser = new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = UserRole.Teacher, RoleApproved = true };
        var expectedCreds = new UserCredsDTO { JWT = expectedJWT, Role = inputUser.Role.ToString(), RoleApproved = inputUser.RoleApproved };

        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(""))
            .ReturnsAsync(inputUser);
        _mockAuthService
            .Setup(auth => auth.GenerateJWTToken(inputUser))
            .Returns(expectedJWT);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.GetJWTByEmailAndPasswordAsync(inputUser.Email, inputUser.Password));
    }

    [Fact]
    public async Task UpdateUserPinCodeAsync_UpdatesPincode_WhenCodeIsFourCharacters()
    {
        // Arrange
        var newPincode = "1234";
        var expectedPincode = "encrypted";
        var id = 1;

        _mockPasswordHandler
            .Setup(handler => handler.EncryptPassword(newPincode))
            .Returns(expectedPincode);
        _mockUserRepository
            .Setup(repo => repo.UpdatePinCodeAsync(expectedPincode, id))
            .ReturnsAsync(expectedPincode);

        // Act
        var result = await _userService.UpdateUserPinCodeAsync(newPincode, id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPincode, result);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    public async Task UpdateUserPinCodeAsync_ThrowException_WhenCodeIsNotFourCharacters(string pin)
    {
        // Arrange
        var expectedPincode = BCrypt.Net.BCrypt.HashPassword(pin);
        var id = 1;

        _mockUserRepository
            .Setup(repo => repo.UpdatePinCodeAsync(expectedPincode, id))
            .ReturnsAsync(expectedPincode);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.UpdateUserPinCodeAsync(pin, id));
    }

    [Fact]
    public async Task GetUserByIdAndPinCodeAsync_ReturnsAUser_WhenGivenProperCredentials()
    {
        // Arrange
        var pin = "1234";
        var encryptedPin = "encrypted";
        var token = "jwtToken";
        var inputUser = new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = UserRole.Teacher, RoleApproved = true };
        var expectedCreds = new UserCredsDTO { JWT = token, Role = inputUser.Role.ToString(), RoleApproved = inputUser.RoleApproved };

        _mockUserRepository
            .Setup(repo => repo.GetPinCodeByIdAsync(inputUser.Id))
            .ReturnsAsync(encryptedPin);
        _mockPasswordHandler
            .Setup(handler => handler.VerifyPassword(pin, encryptedPin))
            .Returns(true);
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(inputUser.Id))
            .ReturnsAsync(inputUser);
        _mockAuthService
            .Setup(auth => auth.GenerateJWTToken(inputUser))
            .Returns(token);

        // Act
        var result = await _userService.GetUserByIdAndPinCodeAsync(inputUser.Id, pin);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCreds.JWT, result.JWT);
        Assert.Equal(expectedCreds.Role, result.Role);
        Assert.Equal(expectedCreds.RoleApproved, result.RoleApproved);
    }

    [Fact]
    public async Task UserHasPinCodeAsync_ChecksValueInRepository()
    {
        // Arrange
        var id = 1;

        _mockUserRepository
            .Setup(repo => repo.HasPinCodeAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UserHasPinCodeAsync(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetUsersNotApprovedAsync_ReturnsUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User
            {
                Id = 1,
                FirstName = "niels",
                LastName = "nielsen",
                Email = "nielsen@example.com",
                Role = UserRole.Teacher,
                Archived = true,
                Password = "",
                RoleApproved = false
            },
            new User
            {
                Id = 2,
                FirstName = "ole",
                LastName = "olsen",
                Email = "olsen@example.com",
                Role = UserRole.Teacher,
                Archived = true,
                Password = "",
                RoleApproved = false
            },
        };

        _mockUserRepository
            .Setup(repo => repo.GetAllNotApprovedAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersNotApprovedAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(users.Count(), result.Count());
        Assert.All(result, user => Assert.Contains(users,
                                                    u => u.Id == user.Id &&
                                                    u.FirstName == user.FirstName &&
                                                    u.LastName == user.LastName &&
                                                    u.Email == user.Email &&
                                                    u.Role == user.Role &&
                                                    u.RoleApproved == user.RoleApproved &&
                                                    u.Archived == user.Archived));
    }

    [Fact]
    public async Task UserUpdateArchivedAsync_UpdatesValueInRepository()
    {
        // Arrange
        var id = 1;
        var archived = true;
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "JohnDoe.Sexy@google.com",
            Role = UserRole.Parent,
            RoleApproved = true,
            Password = "123"
        };

        _mockUserRepository
            .Setup(repo => repo.UpdateArchivedAsync(id, archived))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(user);

        // Act
        var result = await _userService.UserUpdateArchivedAsync(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UserUpdateRoleApprovedAsync_UpdatesValueInRepository()
    {
        // Arrange
        var id = 1;
        var roleApproved = false;

        _mockUserRepository
            .Setup(repo => repo.UpdateRoleApprovedAsync(id, roleApproved))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UserUpdateRoleApprovedAsync(id, roleApproved);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UserSelectAllNotArchivedAsync_ReturnsUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
    {
        new() { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = UserRole.Teacher, RoleApproved = true },
        new() { Id = 2, FirstName = "ole", LastName = "olsen", Email = "olsen@example.com", Password = "password", Role = UserRole.Parent, RoleApproved = true },
    };

        _mockUserRepository
            .Setup(repo => repo.SelectAllNotArchivedAsync())
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.UserSelectAllNotArchivedAsync();

        // Assert
        Assert.Equal(expectedUsers.Count, result.Count());

        Assert.All(result, user => Assert.NotNull(user));

        Assert.Equal(
            expectedUsers.Select(u => new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Password,
                u.Role,
                u.RoleApproved
            }),
            result.Select(u =>
            {
                var user = u!;
                return new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Password,
                    user.Role,
                    user.RoleApproved
                };
            })
        );

    }


    [Fact]
    public async Task GetLoggedInUserAsync_ReturnsAUser()
    {
        // Arrange
        var expectedUser = new User
        {
            Id = 1,
            FirstName = "niels",
            LastName = "nielsen",
            Email = "nielsen@example.com",
            Role = UserRole.Teacher,
            Archived = true,
            Password = "",
            RoleApproved = false
        };
        _mockUserRepository
            .Setup(repo => repo.GetLoggedInAsync(expectedUser.Id))
            .ReturnsAsync(expectedUser);


        // Act
        var result = await _userService.GetLoggedInUserAsync(expectedUser.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.FirstName, result.FirstName);
        Assert.Equal(expectedUser.LastName, result.LastName);
        Assert.Equal(expectedUser.Email, result.Email);
        Assert.Equal(expectedUser.Role, result.Role);
        Assert.Equal(expectedUser.RoleApproved, result.RoleApproved);
        Assert.Equal(expectedUser.Archived, result.Archived);
    }

    [Fact]
    public async Task UpdateUserLoggedInAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        var updateUser = new UserUpdateLoggedInDTO()
        {
            FirstName = "niels",
            LastName = "nielsen",
            Email = "nielsen@example.com",
        };

        _mockUserRepository
            .Setup(repo => repo.UpdateLoggedInAsync(expectedId, updateUser))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _userService.UpdateUserLoggedInAsync(expectedId, updateUser);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        var password = "password";
        var encryptedPassword = "encrypted password";

        _mockPasswordHandler
            .Setup(handler => handler.EncryptPassword(password))
            .Returns(encryptedPassword);

        _mockUserRepository
            .Setup(repo => repo.UpdatePasswordAsync(encryptedPassword, expectedId))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _userService.UpdateUserPasswordAsync(password, expectedId);

        // Assert
        Assert.Equal(expectedId, result);
    }


    [Fact]
    public async Task CreateChildrenUserAsync_CreatesParentRelation()
    {
        // Arrange
        var expectedId = 1;
        var mail = "child@example.com";
        var parentId = 2;
        var password = "password";
        var encryptedPassword = "encryptedPassword";

        var newUser = new UserCreateChildDTO
        {
            FirstName = "lisa",
            LastName = "child",
            Email = mail,
            Password = password,
            ClassId = 3
        };

        var parentUser = new User
        {
            Id = parentId,
            FirstName = "parent",
            LastName = "parent",
            Email = "parent@example.com",
            Password = "password",
            Role = UserRole.Parent,
            RoleApproved = true
        };

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(parentId))
            .ReturnsAsync(parentUser);

        _mockUserRepository
            .Setup(repo => repo.EmailExistsAsync(mail))
            .ReturnsAsync(false);

        _mockPasswordHandler
            .Setup(handler => handler.EncryptPassword(password))
            .Returns(encryptedPassword);

        _mockUserRepository
            .Setup(repo => repo.InsertAsync(It.IsAny<User>()))
            .ReturnsAsync(expectedId);

        _mockChildrenRepository
            .Setup(repo => repo.InsertAsync(It.IsAny<Children>()))
            .ReturnsAsync(expectedId);

        _mockChildrenRepository
            .Setup(repo => repo.AddUserToChildAsync(parentId, expectedId))
            .ReturnsAsync(1);

        // Act
        var result = await _userService.CreateChildrenUserAsync(newUser, parentId);

        // Assert
        Assert.Equal(expectedId, result);

        _mockUserRepository.Verify(repo =>
            repo.InsertAsync(It.Is<User>(user =>
                user.FirstName == newUser.FirstName &&
                user.LastName == newUser.LastName &&
                user.Email == newUser.Email &&
                user.Password == encryptedPassword &&
                user.Role == UserRole.Child &&
                user.RoleApproved == parentUser.RoleApproved
            )), Times.Once);

        _mockChildrenRepository.Verify(repo =>
            repo.InsertAsync(It.Is<Children>(child =>
                child.ChildId == expectedId &&
                child.FirstName == newUser.FirstName &&
                child.LastName == newUser.LastName &&
                child.ClassId == newUser.ClassId
            )), Times.Once);

        _mockChildrenRepository.Verify(repo =>
            repo.AddUserToChildAsync(parentId, expectedId),
            Times.Once);
    }


    [Fact]
    public async Task CreateChildrenUserAsync_ThrowsException_WhenParentDoesNotExist()
    {
        // Arrange
        var invalidParentId = 999;

        var newUser = new UserCreateChildDTO
        {
            FirstName = "lisa",
            LastName = "child",
            Email = "child@example.com",
            Password = "password"
        };

        // Simulate missing parent
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(invalidParentId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateChildrenUserAsync(newUser, invalidParentId));

        // Verify that no user or child was created
        _mockUserRepository.Verify(repo =>
            repo.InsertAsync(It.IsAny<User>()), Times.Never);

        _mockChildrenRepository.Verify(repo =>
            repo.InsertAsync(It.IsAny<Children>()), Times.Never);

        _mockChildrenRepository.Verify(repo =>
            repo.AddUserToChildAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

}