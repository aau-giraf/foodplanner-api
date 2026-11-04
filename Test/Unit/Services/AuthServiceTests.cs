using FoodplannerModels.Account;
using FoodplannerServices.Auth;
using FoodplannerServices.Secret;
using Microsoft.Extensions.Configuration;
using Moq; //Library used for mocking


namespace Test.Service;

public class AuthServiceTests{

//The one who creates a token
private const string Issuer = "TestIssuer";
//The one who receives the token
private const string Audience = "TestAudience";

//Initializes a test user
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
    
//Helper method to create AuthService for GenerateJWTSecret
private AuthService CreateAuthService(int secretLength)
{
    //Creates a secret with the length of 8 bits
    var secret = new string('A', secretLength);

    //Mocks interface SecretLoader
    var secretLoader = new Mock<ISecretLoader>();

    //Makes sure secretLoader returns a secret when GetSecret receives input "JWT_SECRET"
    secretLoader.Setup(x => x.GetSecret("JWT_SECRET")).Returns(secret);

    //Creates a configuration with 3 settings in memory 
    //Configuration needed for AuthService 
    var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> {{"ApplicationSettings:JWT_Issuer", Issuer}, {"ApplicationsSettings:JWT_Audience", Audience}, {"ApplicationSettings:JwtExpireDays", "30"}}).Build();

    //.Object indicates that we use the mocked version
    return new AuthService(configuration, secretLoader.Object);
} 


[Theory]
// Bits used for the length of the secret
[InlineData(32)] //256 bit, limit
[InlineData(33)] //264 bits, above limit
[InlineData(64)] //512 bits
public void GenerateJWTToken_ShouldPass_SecretIsEqualOrAbove256(int secretLength)
{
    //Arrange
    var AuthService = CreateAuthService(secretLength);

    //Act
    var token = AuthService.GenerateJWTToken(user); ; 

    //Assert 
    Assert.IsType<string>(token);
}

[Theory]
[InlineData(16)] //128 bits
[InlineData(31)] //248 bits
    public void GenerateJWTToken_ShouldFail_SecretIsTooShort(int secretLength)
{
    //Arrange
    var AuthService = CreateAuthService(secretLength);

    //Act & Assert
    //Throws an exception if secret length is below 256 bits else 
    var exception = Assert.Throws<InvalidOperationException>(()=> {var token = AuthService.GenerateJWTToken(user); });

    //Makes sure the exception message is being thrown 
    Assert.Contains($"JWT_SECRET must be at least 256 bits long. Current size: {secretLength*8}", exception.Message, StringComparison.OrdinalIgnoreCase);
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
