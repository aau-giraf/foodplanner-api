using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FoodplannerApi.Controller;
using FoodplannerServices.Auth;
using FoodplannerServices.Account;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Builder;
using FoodplannerModels.Auth;
using FoodplannerModels.Codes;

namespace Test.LunchboxTest.Controller;

public class UsersControllerTests
{
    [Fact]
    public async Task EmailExists_ReturnsOkObjectResult_WhenEmailIsValid()
    {
        // Arrange
        var email = "test@example.com";

        var mockUserService = new Mock<IUserService>();
        var authService = new Mock<IAuthService>();
        var otpService = new Mock<IOneTimePasswordService>();
        mockUserService
            .Setup(s => s.UserEmailExistsAsync(email))
            .ReturnsAsync(true);


        var usersController = new UsersController(mockUserService.Object, authService.Object, otpService.Object);

        // Act
        var result = await usersController.EmailExists(email);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic value = okResult.Value!;
        bool exists = (bool)value.GetType().GetProperty("EmailExists").GetValue(value);
        Assert.True(exists);
    }


    [Fact]
    public async Task EmailExists_ReturnsBadRequest_WhenEmailIsNullOrEmpty()
    {
        // Arrange
        string email = ""; // could also test null

        var mockUserService = new Mock<IUserService>();
        var authService = new Mock<IAuthService>();
        var otpService = new Mock<IOneTimePasswordService>();

        var usersController = new UsersController(mockUserService.Object, authService.Object, otpService.Object);

        // Act
        var result = await usersController.EmailExists(email);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        dynamic value = badRequest.Value!;
        Assert.IsType<string[]>(value.Message);
        var messages = (string[])value.Message;

        Assert.Contains("Email skal angives", messages);

    }

    [Fact]
    public async Task EmailExists_ReturnsOkObjectResult_WhenEmailDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";

        var mockUserService = new Mock<IUserService>();
        var authService = new Mock<IAuthService>();
        var otpService = new Mock<IOneTimePasswordService>();
        mockUserService
            .Setup(s => s.UserEmailExistsAsync(email))
            .ReturnsAsync(false);


        var usersController = new UsersController(mockUserService.Object, authService.Object, otpService.Object);

        // Act
        var result = await usersController.EmailExists(email);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic value = okResult.Value!;
        bool exists = (bool)value.GetType().GetProperty("EmailExists").GetValue(value);
        Assert.False(exists);
    }
}
