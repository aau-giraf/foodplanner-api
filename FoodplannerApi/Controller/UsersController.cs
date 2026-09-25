using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Auth;
using FoodplannerModels.Codes;

namespace FoodplannerApi.Controller;

public class UsersController(IUserService userService, IAuthService authService, IOneTimePasswordService oneTimePasswordService) : BaseController
{

    // URL: api/Users/GetBearerTest
    // Generates a JWT token for development purposes with a hardcoded user
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBearerTest()
    {
        //Roles can be: Admin, Child, Teacher, Parent
        var user = new User
        {
            Id = 27,
            FirstName = "test",
            LastName = "test",
            Email = "user@test.com",
            Password = "test",
            Role = UserRole.Admin,
            RoleApproved = true
        };
        var token = authService.GenerateJWTToken(user);
        return Ok(token);
    }

    // URL: api/Users/Create
    // Creates a new non-child user from UserCreateDTO
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UserCreateDTO userCreateDto)
    {
        // Check if the role is Child, which is not allowed for this endpoint
        if (Enum.TryParse<UserRole>(userCreateDto.Role, true, out var parsedRole) && parsedRole == UserRole.Child)
        {
            return BadRequest(new ErrorResponse { Message = ["Children must be created using the CreateUserChildren endpoint."] });
        }
        
        // Validate the model state to ensure the incoming data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Calls the service to create a new user and returns the result
        try
        {
            var id = await userService.CreateUserAsync(userCreateDto);
            if (id > 0)
            {
                return Created(string.Empty, id);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Email = [e.Message] });
        }
    }
    
    // URL: api/Users/CreateUserChildren
    // Creates a new child user from UserCreateChildDTO, which is associated with the parent user of authorization token
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUserChildren([FromHeader(Name = "Authorization")] string token, [FromBody] UserCreateChildDTO userCreateChildDto)
    {
        // Validate the model state to ensure the incoming data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int parentId))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Calls the service to create a new child user associated with the user ID
            var id = await userService.CreateChildrenUserAsync(userCreateChildDto, parentId);
            if (id > 0)
            {
                return Created(string.Empty, id);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Email = [e.Message] });
        }
    }

    // URL: api/Users/Login
    // Authenticates a non-child user with LoginDTO and returns a JWT token if successful
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        // Validate the model state to ensure the incoming data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var result = await userService.GetJWTByEmailAndPasswordAsync(login.Email, login.Password);
            
            //TODO: Handle usecase for parent using one time password
            /*if (!string.IsNullOrEmpty(user.Code) && result != null)
            {
                if (Enum.TryParse<UserRole>(result.Role, true, out var parsedRole) && parsedRole == UserRole.Child)
                {
                    return BadRequest(new ErrorResponse { Message = ["Børn må ikke laves med dette endpoint, istedet skal LoginChild bruges."] });
                }
                var code = await oneTimePasswordService.GetOneTimePassword(user.Code);
                code.Used = true;
                code.UsedByUser = int.Parse(authService.RetrieveIdFromJwtTokenNoBearer(result.JWT));

                if (await oneTimePasswordService.UpdateOneTimePassword(code) == 0)
                    return BadRequest(new ErrorResponse { Message = ["Fejlede i at opdatere engangskode"] });
                if (await oneTimePasswordService.RedeemOneTimePassword(code.Code) == 0)
                    return BadRequest(new ErrorResponse{ Message = ["Fejlede i at indløse engangskode"] });
            }*/

            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new ErrorResponse { Message = ["Email or password is wrong"] });
        }
        catch
        {
            return BadRequest(new ErrorResponse { Message = ["Email or password is wrong"] });
        }
    }

    // URL: api/Users/LoginChild
    // Authenticates a child user with LoginChild and returns a JWT token if successful
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LoginChild([FromBody] LoginChild user)
    {
        // Validate the model state to ensure the incoming data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            // Calls the service to authenticate the child user and retrieve a JWT token
            var result = await userService.GetJWTByEmailAsync(user.Email);
            if (!string.IsNullOrEmpty(user.Code) && result != null)
            {
                // If a one-time password is provided, redeem it for the user
                int id = int.Parse(authService.RetrieveIdFromJwtTokenNoBearer(result.JWT));
                if (await oneTimePasswordService.RedeemOneTimePassword(user.Code, id) == 0)
                    return BadRequest("Failed while trying to redeem the one time code");
            }

            // Return the JWT token if authentication is successful, otherwise return a bad request response
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new ErrorResponse { Message = ["Email or password is wrong"] });
        }
        catch
        {
            return BadRequest(new ErrorResponse { Message = ["Email or password is wrong"] });
        }
    }

    // URL: api/Users/UpdatePinCode
    // Updates the pin code for the authenticated user
    [HttpPut]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePinCode([FromHeader(Name = "Authorization")] string token, [FromBody] Pincode pincode)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Calls the service to update the pin code for the user
            var result = await userService.UpdateUserPinCodeAsync(pincode.PinCode, id);
            if (result.Length > 0)
            {
                return Created();
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Users/CheckPinCode
    // Checks if the provided pin code matches the authenticated users pin code
    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckPinCode([FromHeader(Name = "Authorization")] string token, [FromBody] Pincode pincode)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Calls the service to check if pin codes match and returns the result
            var result = await userService.GetUserByIdAndPinCodeAsync(id, pincode.PinCode);
            return Ok(result);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Users/EmailExists
    // Checks if the provided email exists in the system
    [HttpGet]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EmailExists([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new ErrorResponse { Message = ["Email is required"] });
        }

        var exists = await userService.UserEmailExistsAsync(email);
        return Ok(new { EmailExists = exists });
    }

    // URL: api/Users/HasPinCode
    // Checks if the authenticated user has a pin code set
    [HttpGet]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HasPinCode([FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Calls the service to check if the user has a pin code and returns the result
            var result = await userService.UserHasPinCodeAsync(id);
            return Ok(new { HasPinCode = result });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Users/GetLoggedIn
    // Retrieves the currently logged-in user based on the authorization token
    [HttpGet]
    [Authorize(Roles = "Child, Parent, Teacher, Admin")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLoggedIn([FromHeader(Name = "Authorization")] string token)
    {
        // Retrieve the user ID from the JWT token
        var idString = authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
        }

        // Calls the service to retrieve the logged-in user and returns the result
        var user = await userService.GetLoggedInUserAsync(id);
        return Ok(user);
    }

    // URL: api/Users/UpdateLoggedIn
    // Updates the currently logged-in user based on the authorization token and UserUpdateLoggedInDTO
    [HttpPut]
    [Authorize(Roles = "Parent, Child,  Teacher, Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLoggedIn([FromHeader(Name = "Authorization")] string token, [FromBody] UserUpdateLoggedInDTO user)
    {
        // Retrieve the user ID from the JWT token
        var idString = authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
        }

        // Calls the service to update the logged-in user and returns the result
        var result = await userService.UpdateUserLoggedInAsync(id, user);
        if (result > 0)
        {
            return Created();
        }
        return NotFound();
    }

    // URL: api/Users/UpdatePassword
    // Updates the password for the currently logged-in user based on the authorization token and Password
    [HttpPut]
    [Authorize(Roles = "Parent, Child,  Teacher, Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePassword([FromHeader(Name = "Authorization")] string token, [FromBody] Password password)
    {
        // Retrieve the user ID from the JWT token
        var idString = authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
        }

        // Calls the service to update the password for the logged-in user and returns the result
        var result = await userService.UpdateUserPasswordAsync(password.password, id);
        if (result > 0)
        {
            return Created();
        }
        return NotFound();
    }

    // URL: api/Users/DeleteLoggedIn
    // Deletes the currently logged-in user based on the authorization token
    [HttpDelete]
    [Authorize(Roles = "Parent, Child, Teacher, Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteLoggedInUser([FromHeader(Name = "Authorization")] string token)
    {
        // Retrieve the user ID from the JWT token
        var idString = authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
        }

        // Calls the service to delete the logged-in user and returns the result
        int result = await userService.DeleteUserAsync(id);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }
}