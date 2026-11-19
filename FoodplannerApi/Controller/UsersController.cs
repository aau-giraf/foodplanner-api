using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Auth;

namespace FoodplannerApi.Controller;

public class UsersController(IUserService userService, IAuthService authService) : BaseController
{
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
            return BadRequest(new ErrorResponseDTO { Message = ["Children must be created using the CreateUserChildren endpoint."] });
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
            return BadRequest(new ErrorResponseDTO { Email = [e.Message] });
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
                return BadRequest(new ErrorResponseDTO { Message = ["Invalid user ID"] });
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
            return BadRequest(new ErrorResponseDTO { Email = [e.Message] });
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
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new ErrorResponseDTO { Message = ["Email or password is wrong"] });
        }
        catch
        {
            return BadRequest(new ErrorResponseDTO { Message = ["Email or password is wrong"] });
        }
    }

    // URL: api/Users/UpdatePinCode/{pincode}
    // Updates the pin code for the authenticated user
    [HttpPut("{pincode}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePinCode([FromHeader(Name = "Authorization")] string token, string pincode)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponseDTO { Message = ["Invalid user ID"] });
            }

            // Calls the service to update the pin code for the user
            var result = await userService.UpdateUserPinCodeAsync(pincode, id);
            if (result.Length > 0)
            {
                return Created();
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponseDTO { Message = [e.Message] });
        }
    }

    // URL: api/Users/CheckPinCode/{pincode}
    // Checks if the provided pin code matches the authenticated users pin code
    [HttpPost("{pincode}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckPinCode([FromHeader(Name = "Authorization")] string token, string pincode)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponseDTO { Message = ["Invalid user ID"] });
            }

            // Calls the service to check if pin codes match and returns the result
            var result = await userService.GetUserByIdAndPinCodeAsync(id, pincode);
            return Ok(result);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponseDTO { Message = [e.Message] });
        }
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
                return BadRequest(new ErrorResponseDTO { Message = ["Invalid user ID"] });
            }

            // Calls the service to check if the user has a pin code and returns the result
            var result = await userService.UserHasPinCodeAsync(id);
            return Ok(new { HasPinCode = result });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponseDTO { Message = [e.Message] });
        }
    }

    // URL: api/Users/GetLoggedIn
    // Retrieves the currently logged-in user based on the authorization token
    [HttpGet]
    [Authorize(Roles = "Child, Parent, Teacher, Admin")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLoggedIn([FromHeader(Name = "Authorization")] string token)
    {
        // Retrieve the user ID from the JWT token
        var idString = authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponseDTO { Message = ["Invalid user ID"] });
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
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLoggedIn([FromHeader(Name = "Authorization")] string token, [FromBody] UserUpdateLoggedInDTO user)
    {
        // Retrieve the user ID from the JWT token
        var idString = authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponseDTO { Message = ["Invalid user ID"] });
        }

        // Calls the service to update the logged-in user and returns the result
        var result = await userService.UpdateUserLoggedInAsync(id, user);
        if (result > 0)
        {
            return Created();
        }
        return NotFound();
    }

    // URL: api/Users/UpdatePassword/{password}
    // Updates the password for the currently logged-in user based on the authorization token and Password
    [HttpPut("{password}")]
    [Authorize(Roles = "Parent, Child,  Teacher, Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePassword([FromHeader(Name = "Authorization")] string token, string password)
    {
        // Retrieve the user ID from the JWT token
        var idString = authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponseDTO { Message = ["Invalid user ID"] });
        }

        // Calls the service to update the password for the logged-in user and returns the result
        var result = await userService.UpdateUserPasswordAsync(password, id);
        if (result > 0)
        {
            return Created();
        }
        return NotFound();
    }
}