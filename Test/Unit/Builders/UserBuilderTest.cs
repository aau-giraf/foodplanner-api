using FoodplannerModels.Account;
using Test.Builder;

namespace Test.BuilderTest;

public class UserBuilderTest
{
    [Fact]
    public void UserBuilder_ReturnsUser_ThatIsNotNull()
    {
        //Arrange
        var builder = new UserBuilder();

        //Act
        var user = builder.Build();

        //Assert
        Assert.NotNull(user);
    }

    [Theory]
    [InlineData("Lars", "Larsen", "LarsLarsen@jysk.dk", "Lars1234", UserRole.Parent, true, "1234", false)]
    [InlineData("Anna", "Andersen", "anna.andersen@email.com", "Anna5678", UserRole.Parent, false, "5678", true)]
    public void UserBuilder_ReturnsUser_WithExpectedValues(string firstName, string lastName, string email,
        string password, UserRole role, bool approved, string pincode, bool archived)
    {
        // Arrange
        var builder = new UserBuilder()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .WithEmail(email)
            .WithPassword(password)
            .WithRole(role)
            .WithRoleApproved(approved)
            .WithPinCode(pincode)
            .WithArchived(archived);

        // Act
        var user = builder.Build();

        // Assert
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(email, user.Email);
        Assert.Equal(password, user.Password);
        Assert.Equal(role, user.Role);
        Assert.Equal(approved, user.RoleApproved);
        Assert.Equal(pincode, user.PinCode);
        Assert.Equal(archived, user.Archived);
    }
}