using FoodplannerModels.Account;

namespace Test.Builder;

public class UserBuilder : User
{
    public UserBuilder()
    {
        this.FirstName = "testFirstName";
        this.LastName = "testLastName";
        this.Email = "test@testing.com";
        this.Password = "Test123!";
        this.Role = "Parent";
    }
    public UserBuilder WithFirstName(string firstName)
    {
        this.FirstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        this.LastName = lastName;
        return this;
    }
    
    public UserBuilder WithEmail(string email)
    {
        this.Email = email;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        this.Password = password;
        return this;
    }

    public UserBuilder WithRole(string role)
    {
        this.Role = role;
        return this;
    }

    public UserBuilder WithRoleApproved(bool roleApproved)
    {
        this.RoleApproved = roleApproved;
        return this;
    }

    public UserBuilder WithPinCode(string pinCode)
    {
        this.PinCode = pinCode;
        return this;
    }

    public UserBuilder WithArchived(bool archived)
    {
        this.Archived = archived;
        return this;
    }

    public User Build()
    {
        return new User
        {
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            Password = this.Password,
            Role = this.Role,
            RoleApproved = this.RoleApproved,
            PinCode = this.PinCode,
            Archived = this.Archived,
        };
    }
}