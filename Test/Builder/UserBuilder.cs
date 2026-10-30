using FoodplannerModels.Account;

namespace Test.Builder;

public class UserBuilder
{
    private string _firstName = "testFirstName";
    private string _lastName = "testLastName";
    private string _email = "test@gtesting.com";
    private string _password = "Test123!";
    private UserRole _role = UserRole.Parent;
    private bool _roleApproved;
    private string? _pinCode;
    private bool _archived;

    public UserBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public UserBuilder WithRole(UserRole role)
    {
        _role = role;
        return this;
    }

    public UserBuilder WithRoleApproved(bool roleApproved)
    {
        _roleApproved = roleApproved;
        return this;
    }

    public UserBuilder WithPinCode(string pinCode)
    {
        _pinCode = pinCode;
        return this;
    }

    public UserBuilder WithArchived(bool archived)
    {
        _archived = archived;
        return this;
    }

    public User Build()
    {
        return new User
        {
            FirstName = _firstName,
            LastName = _lastName,
            Email = _email,
            Password = _password,
            Role = _role,
            RoleApproved = _roleApproved,
            PinCode = _pinCode,
            Archived = _archived,
        };
    }
}