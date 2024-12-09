using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Moq;
using AutoMapper;
using FoodplannerModels.Auth;

namespace Test.Service;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IChildrenRepository> _mockChildrenRepository;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockChildrenRepository = new Mock<IChildrenRepository>();
        _mockAuthService = new Mock<IAuthService>();
        _mockMapper = new Mock<IMapper>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockAuthService.Object,
            _mockChildrenRepository.Object
        );
    }
    
    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var expectedUsers = new List<UserDTO>
        {
            new UserDTO { Id = 1, First_name = "niels", Last_name = "nielsen", Email = "nielsen@example.com", Role = "Teacher", Archived = true },
            new UserDTO { Id = 2, First_name = "ole", Last_name = "olsen", Email = "olsen@example.com", Role = "Parent", Archived = true },
        };
        _mockUserRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedUsers);
    

        _mockMapper.Setup(lu => lu.Map<List<UserDTO>>(It.IsAny<List<UserDTO>>()))
            .Returns(expectedUsers);
        
        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUsers.Count, result.Count());
        Assert.All(result, user => Assert.Contains(expectedUsers, 
                                                    u => u.Id == user.Id && 
                                                    u.First_name == user.First_name && 
                                                    u.Last_name == user.Last_name &&
                                                    u.Email == user.Email &&
                                                    u.Role == user.Role &&
                                                    u.Archived == user.Archived));
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsAUser_WhenIdIsValid()
    {
        // Arrange
        var expectedUser = new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = "Teacher", RoleApproved = true };
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(expectedUser.Id))
            .ReturnsAsync(expectedUser);
        
        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result);
    }

    [Fact]
    public async Task CreateUserAsync_CreatesANewUser()
    {
        // Arrange
        var expectedId = 1;
        var mail = "nielsen@example.com";
        var newUser = new UserCreateDTO { FirstName = "niels", LastName = "nielsen", Email = mail, Password = "password", Role = "Teacher" };
        var mappedUser = new User { Id = expectedId, FirstName = "niels", LastName = "nielsen", Email = mail, Password = "password", Role = "Teacher", RoleApproved = true };
        
        _mockMapper
            .Setup(mapper => mapper.Map<User>(newUser))
            .Returns(mappedUser);
        _mockUserRepository
            .Setup(repo => repo.EmailExistsAsync(mail))
            .ReturnsAsync(false);
        _mockUserRepository
            .Setup(repo => repo.InsertAsync(mappedUser))
            .ReturnsAsync(mappedUser.Id);
        
        // Act
        var result = await _userService.CreateUserAsync(newUser);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsAnException_WhenEmailAlreadyExists()
    {
        // Arrange
        var expectedId = 1;
        var mail = "nielsen@example.com";
        var newUser = new UserCreateDTO { FirstName = "niels", LastName = "nielsen", Email = mail, Password = "password", Role = "Teacher" };
        var mappedUser = new User { Id = expectedId, FirstName = "niels", LastName = "nielsen", Email = mail, Password = "password", Role = "Teacher", RoleApproved = true };
        
        _mockMapper
            .Setup(mapper => mapper.Map<User>(newUser))
            .Returns(mappedUser);
        _mockUserRepository
            .Setup(repo => repo.EmailExistsAsync(mail))
            .ReturnsAsync(true);
        _mockUserRepository
            .Setup(repo => repo.InsertAsync(mappedUser))
            .ReturnsAsync(mappedUser.Id);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(newUser));
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        var inputUser = new User { Id = expectedId, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = "Teacher", RoleApproved = true };

        _mockUserRepository
            .Setup(repo => repo.UpdateAsync(inputUser))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _userService.UpdateUserAsync(inputUser);
        
        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task DeleteUserAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        var inputUser = new User { Id = expectedId, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = "Teacher", RoleApproved = true };

        _mockUserRepository
            .Setup(repo => repo.DeleteAsync(expectedId))
            .ReturnsAsync(expectedId);
        
        // Act
        var result = await _userService.UpdateUserAsync(inputUser);
        
        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task GetJWTByEmailAndPasswordAsync_ReturnsUser_WithMatchingCredentials()
    {
        // Arrange
        var expectedJWT = "jwtToken";
        var inputUser = new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = "Teacher", RoleApproved = true };
        var expectedCreds = new UserCredsDTO { JWT = expectedJWT, Role = inputUser.Role, RoleApproved = inputUser.RoleApproved };
        
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(inputUser.Email))
            .ReturnsAsync(inputUser);
        _mockAuthService
            .Setup(auth => auth.GenerateJWTToken(inputUser))
            .Returns(expectedJWT);
        
        // Act
        var result = await _userService.GetJWTByEmailAndPasswordAsync(inputUser.Email, inputUser.Password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCreds, result);
    }

    [Theory]
    [InlineData("Parent")]
    [InlineData("Child")]
    public async Task GetJWTByEmailAndPasswordAsync_ReturnsUser_WithChildCredentials(string inputUserRole)
    {
        // Arrange
        var expectedJWT = "jwtToken";
        var inputUser = new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = inputUserRole, RoleApproved = true };
        var expectedCreds = new UserCredsDTO { JWT = expectedJWT, Role = "Child", RoleApproved = inputUser.RoleApproved };
        
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(inputUser.Email))
            .ReturnsAsync(inputUser);
        _mockAuthService
            .Setup(auth => auth.GenerateJWTToken(inputUser))
            .Returns(expectedJWT);
        
        // Act
        var result = await _userService.GetJWTByEmailAndPasswordAsync(inputUser.Email, inputUser.Password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCreds, result);
    }

    [Fact]
    public async Task GetJWTByEmailAndPasswordAsync_ThrowsAnException_WhenGivenWrongEmailOrPassword()
    {
        // Arrange
        var expectedJWT = "jwtToken";
        var inputUser = new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = "Teacher", RoleApproved = true };
        var expectedCreds = new UserCredsDTO { JWT = expectedJWT, Role = inputUser.Role, RoleApproved = inputUser.RoleApproved };
        
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(null))
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
        var expectedPincode = BCrypt.Net.BCrypt.HashPassword(newPincode);
        var id = 1;

        _mockUserRepository
            .Setup(repo => repo.UpdatePinCodeAsync(expectedPincode, id))
            .ReturnsAsync(expectedPincode);
        
        // Act
        var result = await _userService.UpdateUserPinCodeAsync(newPincode, id);

        // Assert
        Assert.NotNull(expectedPincode);
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
        var token = "jwtToken";
        var inputUser = new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = "Teacher", RoleApproved = true };
        var expectedCreds = new UserCredsDTO { JWT = token, Role = inputUser.Role, RoleApproved = inputUser.RoleApproved };

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(inputUser.Id))
            .ReturnsAsync(inputUser);
        _mockUserRepository
            .Setup(repo => repo.GetPinCodeByIdAsync(inputUser.Id))
            .ReturnsAsync(pin);
        _mockAuthService
            .Setup(auth => auth.GenerateJWTToken(inputUser))
            .Returns(token);
        
        // Act
        var result = await _userService.GetUserByIdAndPinCodeAsync(inputUser.Id, pin);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCreds, result);
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
        var expectedUsers = new List<UserDTO>
        {
            new UserDTO { Id = 1, First_name = "niels", Last_name = "nielsen", Email = "nielsen@example.com", Role = "Teacher", Archived = true },
            new UserDTO { Id = 2, First_name = "ole", Last_name = "olsen", Email = "olsen@example.com", Role = "Parent", Archived = true },
        };
        _mockUserRepository
            .Setup(repo => repo.GetAllNotApprovedAsync())
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.GetUsersNotApprovedAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUsers.Count, result.Count());
        Assert.All(result, user => Assert.Contains(expectedUsers, 
                                                    u => u.Id == user.Id && 
                                                    u.First_name == user.First_name && 
                                                    u.Last_name == user.Last_name &&
                                                    u.Email == user.Email &&
                                                    u.Role == user.Role &&
                                                    u.Archived == user.Archived));
    }

    [Fact]
    public async Task UserUpdateArchivedAsync_UpdatesValueInRepository()
    {
        // Arrange
        var id = 1;

        _mockUserRepository
            .Setup(repo => repo.UpdateArchivedAsync(id))
            .ReturnsAsync(true);

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
            new User { Id = 1, FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com", Password = "password", Role = "Teacher", RoleApproved = true },
            new User { Id = 2, FirstName = "ole", LastName = "olsen", Email = "olsen@example.com", Password = "password", Role = "Parent", RoleApproved = true },
        };
        _mockUserRepository
            .Setup(repo => repo.SelectAllNotArchivedAsync())
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.UserSelectAllNotArchivedAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUsers.Count, result.Count());
        Assert.All(result, user => Assert.Contains(expectedUsers, 
                                                    u => u.Id == user.Id && 
                                                    u.FirstName == user.FirstName && 
                                                    u.LastName == user.LastName &&
                                                    u.Email == user.Email &&
                                                    u.Password == user.Password &&
                                                    u.Role == user.Role &&
                                                    u.RoleApproved == user.RoleApproved));
    }

    [Fact]
    public async Task GetLoggedInUserAsync_ReturnsAUser()
    {
        // Arrange
        var expectedUser = new UserDTO { Id = 1, First_name = "niels", Last_name = "nielsen", Email = "nielsen@example.com", Role = "Teacher", Archived = true };
        _mockUserRepository
            .Setup(repo => repo.GetLoggedInAsync(expectedUser.Id))
            .ReturnsAsync(expectedUser);
        
        // Act
        var result = await _userService.GetLoggedInUserAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result);
    }

    [Fact]
    public async Task UpdateUserLoggedInAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        var updateUser = new UserUpdateDTO { FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com" };

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
        var updateUser = new UserUpdateDTO { FirstName = "niels", LastName = "nielsen", Email = "nielsen@example.com" };

        _mockUserRepository
            .Setup(repo => repo.UpdatePasswordAsync(It.IsAny<string>(), expectedId))
            .ReturnsAsync(expectedId);
        
        // Act
        var result = await _userService.UpdateUserLoggedInAsync(expectedId, updateUser);

        // Assert
        Assert.Equal(expectedId, result);
    }
}