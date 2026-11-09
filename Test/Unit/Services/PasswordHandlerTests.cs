using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Moq;
using AutoMapper;
using FoodplannerModels.Auth;

namespace Test.Unit.Service;

public class PasswordhandlerTests
{
    private readonly PasswordHandler _passwordHandler;

    public PasswordhandlerTests()
    {
        _passwordHandler = new PasswordHandler();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void EncryptPassword_CreatesNewEncryptedPassword()
    {
        // Arrange
        var password = "password";
        
        // Act
        var result = _passwordHandler.EncryptPassword(password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.NotEqual(password, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void VerifyPassword_VerifiesEncryptedPassword()
    {
        // Arrange
        var password = "password";
        var encryptedPassword = _passwordHandler.EncryptPassword(password);
        
        // Act
        var result = _passwordHandler.VerifyPassword(password, encryptedPassword);

        // Assert
        Assert.True(result);
    }
}