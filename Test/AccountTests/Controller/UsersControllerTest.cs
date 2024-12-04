using Moq;
using FoodplannerApi.Controller;
using FoodplannerApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Builder;
using FoodplannerServices.Account;
using FoodplannerModels.Auth;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;



namespace Test.Controller;

public class UsersControllerTests
{

    [Fact]
    public async Task GetBearerTest_ReturnsOkObjectResult()
    {
        //arrange
        var mockUserService = new Mock<IUserService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        var controller = new UsersController(mockUserService.Object, authService);


        var result = await controller.GetBearerTest();

        //act
        var okResult = Assert.IsType<OkObjectResult>(result);
        //assert
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResult_WhenUserIsValid()
    {
        //arrange
        var mockUserService = new Mock<IUserService>();
        var userCreateDto = new UserCreateDTO
        {
            FirstName = "test",
            LastName = "test",
            Email = "user@test.com",
            Password = "test",
            Role = "Teacher",
        };

        mockUserService
            .Setup(service => service.CreateUserAsync(userCreateDto))
            .ReturnsAsync(1);

        var usersController = new UsersController(mockUserService.Object, null);

        //act
        var result = await usersController.Create(userCreateDto);

        //assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(1, createdResult.Value);
    }


    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUserNotCreated()
    {
        //arrange
        var mockUserService = new Mock<IUserService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        var userCreateDTO = new UserCreateDTO
        {
            FirstName = "test",
            LastName = "test",
            Email = "user@test.com",
            Password = "test",
            Role = "Teacher",
        };

        mockUserService
            .Setup(s => s.CreateUserAsync(userCreateDTO))
            .ReturnsAsync(0);

        var controller = new UsersController(mockUserService.Object, authService);

        //act
        var result = await controller.Create(userCreateDTO);

        //assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Create_UserThrowsException_ReturnsBadRequest()
    {
        //arrange
        var mockUserService = new Mock<IUserService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        var userCreateDto = new UserCreateDTO
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "password123",
            Role = "Parent",
        };

        mockUserService
            .Setup(s => s.CreateUserAsync(It.IsAny<UserCreateDTO>()))
            .ThrowsAsync(new InvalidOperationException("An error occurred while creating the user"));

        var controller = new UsersController(mockUserService.Object, authService);

        // Act
        var result = await controller.Create(userCreateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("An error occurred while creating the user", errorResponse.Email[0]);
    }


    [Fact]
    public async Task Login_ReturnsOkObjectResult_WhenValidCredentials()
    {
        var mockUserService = new Mock<IUserService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        var login = new Login
        {
            Email = "test1234@123456789.com",
            Password = "wrongPassword"
        };

        mockUserService
            .Setup(s => s.GetJWTByEmailAndPasswordAsync(login.Email, login.Password))
            .ReturnsAsync(new UserCredsDTO
            {
                JWT = "jwt-token",
                Role = "Teacher",
                RoleApproved = true
            });

        var controller = new UsersController(mockUserService.Object, authService);

        var result = await controller.Login(login);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var userCredsDTO = Assert.IsType<UserCredsDTO>(okResult.Value);
        Assert.Equal("jwt-token", userCredsDTO.JWT);
    }

    [Fact]
    public async Task Login_Failure_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var mockUserService = new Mock<IUserService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        var loginDto = new Login
        {
            Email = "invalid@example.com",
            Password = "wrongpassword"
        };

        mockUserService
            .Setup(s => s.GetJWTByEmailAndPasswordAsync(loginDto.Email, loginDto.Password))
            .ReturnsAsync((UserCredsDTO)null);

        var controller = new UsersController(mockUserService.Object, authService);

        // Act
        var result = await controller.Login(loginDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Email eller password er forkert", errorResponse.Message[0]);
    }


    [Fact]
    public async Task Login_ExceptionHandling_ReturnsBadRequest()
    {
        var mockUserService = new Mock<IUserService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        var loginDto = new Login
        {
            Email = "test@example.com",
            Password = "password123"
        };

        mockUserService
            .Setup(s => s.GetJWTByEmailAndPasswordAsync(loginDto.Email, loginDto.Password))
            .ThrowsAsync(new InvalidOperationException("An unexpected error occurred"));

        var controller = new UsersController(mockUserService.Object, authService);

        var result = await controller.Login(loginDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Email eller password er forkert", errorResponse.Message[0]);
    }

    [Fact]
    public async Task UpdatePinCode_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var mockUserService = new Mock<IUserService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);

        var token = authService.GenerateJWTToken(new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "passwordTester",
            Role = "Parent",
            RoleApproved = true
        });

        var pincode = new Pincode
        {
            PinCode = "1234"
        };


        mockUserService
            .Setup(s => s.UpdateUserPinCodeAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync("PinCodeUpdatedSuccessfully");

        var controller = new UsersController(mockUserService.Object, authService);

        // Act
        var result = await controller.UpdatePinCode($"Bearer {token}", pincode);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task UpdatePinCode_ReturnsCreatedResult_WhenPinCodeIsUpdated()
    {
        // Arrange
        var mockUserService = new Mock<IUserService>();
        var mockAuthService = new Mock<IAuthService>();

        var validToken = "valid-token";
        var userId = 1;

        var pincode = new Pincode { PinCode = "1234" };

        mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(It.Is<string>(token => token == $"Bearer {validToken}")))
            .Returns(userId.ToString());

        mockUserService
            .Setup(service => service.UpdateUserPinCodeAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync("PinCodeUpdatedSuccessfully");

        var controller = new UsersController(mockUserService.Object, mockAuthService.Object);

        // Act
        var result = await controller.UpdatePinCode($"Bearer {validToken}", pincode);

        // Assert
        Assert.NotNull(result);
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
    }





    [Fact]
    public async Task HasPinCode_ReturnsOkObjectResult_WithPinCodeFalse_WhenUserDoesNotHavePinCode()
    {
        //arrange
        var mockUserService = new Mock<IUserService>();
        var authService = new AuthService(WebApplication.CreateBuilder().Configuration);


        var token = authService.GenerateJWTToken(new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "passwordTester",
            Role = "Parent",
            RoleApproved = true
        });


        mockUserService
            .Setup(s => s.UserHasPinCodeAsync(1))
            .ReturnsAsync(false);

        var controller = new UsersController(mockUserService.Object, authService);

        //act
        var result = await controller.HasPinCode($"Bearer {token}");


        var okResult = Assert.IsType<OkObjectResult>(result);

        var response = okResult.Value as object;

        //assert
        Assert.NotNull(response);
        var hasPinCode = (bool)okResult.Value.GetType().GetProperty("HasPinCode").GetValue(okResult.Value, null);

        Assert.False(hasPinCode);
    }

    [Fact]
    public async Task CheckPinCode_ValidTokenAndCorrectPin_ReturnsOk()
    {
        //arrange
        var token = "valid-jwt-token";
        var pinCode = new Pincode { PinCode = "1234" };
        var userId = 27;

        var mockUserService = new Mock<IUserService>();
        var authService = new Mock<IAuthService>();

        var _controller = new UsersController(mockUserService.Object, authService.Object);

        var userCredsDTO = new UserCredsDTO
        {
            JWT = "jwt-token",
            Role = "Child",
            RoleApproved = true
        };

        mockUserService.Setup(service => service.GetUserByIdAndPinCodeAsync(userId, pinCode.PinCode)).ReturnsAsync(userCredsDTO);

        authService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());

        // Act
        var result = await _controller.CheckPinCode(token, pinCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(userCredsDTO, okResult.Value);
    }


    [Fact]
    public async Task CheckPinCode_InvalidToken_ReturnsBadRequest()
    {
        //arrange
        var token = "invalid-jwt-token";
        var pinCode = new Pincode { PinCode = "1234" };
        var userId = 27;

        var mockUserService = new Mock<IUserService>();
        var authService = new Mock<IAuthService>();

        var _controller = new UsersController(mockUserService.Object, authService.Object);

        authService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Throws(new InvalidOperationException("Invalid token"));

        // Act
        var result = await _controller.CheckPinCode(token, pinCode);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }



    [Fact]
    public async Task CheckPinCode_IncorrectPin_ReturnsBadRequest()
    {
        //arrange
        var token = "valid-jwt-token";
        var pinCode = new Pincode { PinCode = "wrongPinBlaBlaBla" };
        var userId = 27;

        var mockUserService = new Mock<IUserService>();
        var authService = new Mock<IAuthService>();

        var _controller = new UsersController(mockUserService.Object, authService.Object);

        var userCredsDTO = new UserCredsDTO
        {
            JWT = "jwt-token",
            Role = "Child",
            RoleApproved = true
        };

        mockUserService.Setup(service => service.GetUserByIdAndPinCodeAsync(userId, pinCode.PinCode)).ReturnsAsync((UserCredsDTO)null);

        authService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());

        // Act
        var result = await _controller.CheckPinCode(token, pinCode);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

}