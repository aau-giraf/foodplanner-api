

using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Castle.Core.Configuration;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerServices.Account;
using FoodplannerServices.Auth;
using FoodplannerServices.Secret;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Moq;


namespace Test.Service;

    public class AuthServiceTests{

    private const string Issuer = "TestIssuer";
    private const string Audience = "TestAudience";

    private readonly User user = new User
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "TestPassword123!",
                Role = UserRole.Admin,
                RoleApproved = true
            };
        
    // Mocks all the interfaces needed 
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ISecretLoader> _mockSecretLoader;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IChildrenRepository> _mockChildrenRepository;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IPasswordHandler> _mockPasswordHandler;
    private readonly UserService _userService;


    public AuthServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockSecretLoader = new Mock<ISecretLoader>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockChildrenRepository = new Mock<IChildrenRepository>();
        _mockAuthService = new Mock<IAuthService>();
        _mockMapper = new Mock<IMapper>();
        _mockPasswordHandler = new Mock<IPasswordHandler>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockAuthService.Object,
            _mockChildrenRepository.Object,
            _mockPasswordHandler.Object
        );

 


    }

    [Fact]
    private AuthService CreateAuthService(int secretLength)
    {
        //Creates a secret with the length of 8 bits
        var secret = new string('A', secretLength);

        var secretsLoader = new Mock<ISecretLoader>();

        secretsLoader.Setup(x => x.GetSecret("JWT_SECRET")).Returns(secret);

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> {{"ApplicationSettings:JWT_Issuer", Issuer}, {"ApplicationsSettings:JWT_Audience", Audience}, {"ApplicationSettings:JwtExpireDays", "30"}}).Build();

        return new AuthService(configuration, secretsLoader.Object);
    }    
    
    // limit 256, >, <, <<, >>, ==, 0

    [Theory]
    [InlineData(32)] //256 bits, used for secretlength 
    public void GenerateJWTToken_SecretLimitIs256(int secretLength)
    {
        //Arrange
        var AuthService = CreateAuthService(secretLength);

        var token = authService.GenerateJWTToken(user);
        Assert.True()
      
    }

    [Theory]
    [InlineData(16)] //128 bits
    [InlineData(31)] //248 bits
        public void GenerateJWTToken_SecretIsTooShort(int secretLength)
    {
        
        //Arrange
        var AuthService = CreateAuthService(secretLength);


        //Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(()=> {var token = AuthService.GenerateJWTToken(user); });
        
      
    }

    }


    /*
---In the future, test for:---

RetrieveIdFromJwtToken:
- Null
- With Bearer prefix
- Without Bearer prefix
- Valid JWT format
- Invalid JWT format

RetrieveIdFromJwtTokenNoBearer:
- Null
- With Bearer prefix
- Without Bearer prefix
- Valid JWT format
- Invalid JWT format

RetrieveRoleFromJwtToken:
- Null
- With Bearer prefix
- Without Bearer prefix
- Valid JWT format
- Invalid JWT format
*/
