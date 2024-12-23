using FoodplannerApi.Helpers;
using FoodplannerModels.Account;
using FoodplannerServices;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FoodplannerApi.Controller;

public class UsersController : BaseController
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    public UsersController(UserService userService, AuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBearerTest()
    {
        //Generates a token for development purposes, Status must be Active.
        //Roles can be: Admin, Child, Teacher, Parent
        var user = new User
        {
            Id = 27,
            FirstName = "test",
            LastName = "test",
            Email = "user@test.com",
            Password = "test",
            Role = "Teacher",
            RoleApproved = true
        };

        var token = _authService.GenerateJWTToken(user);

        return Ok(token);
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] UserCreateDTO userCreate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var id = await _userService.CreateUserAsync(userCreate);
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

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] Login user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _userService.GetJWTByEmailAndPasswordAsync(user.Email, user.Password);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new ErrorResponse { Message = ["Email eller password er forkert"] });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = ["Email eller password er forkert"] });
        }
    }

    [HttpPut]
    [Authorize(Roles = "Child, Parent")]
    public async Task<IActionResult> UpdatePinCode([FromHeader(Name = "Authorization")] string token, [FromBody] Pincode pincode)
    {
        try
        {
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
            }
            var result = await _userService.UpdateUserPinCodeAsync(pincode.PinCode, id);
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

    [HttpPost]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckPinCode([FromHeader(Name = "Authorization")] string token, [FromBody] Pincode pincode)
    {
        try
        {
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
            }
            var result = await _userService.GetUserByIdAndPinCodeAsync(id, pincode.PinCode);

            return Ok(result);

            //return BadRequest(new ErrorResponse {Message = ["Forkert pinkode"]});
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Child, Parent")]
    public async Task<IActionResult> HasPinCode([FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
            }
            var result = await _userService.UserHasPinCodeAsync(id);
            return Ok(new { HasPinCode = result });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Child, Parent, Teacher, Admin")]
    public async Task<IActionResult> GetLoggedIn([FromHeader(Name = "Authorization")] string token)
    {

        var idString = _authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
        }
        var user = await _userService.GetLoggedInUserAsync(id);
        return Ok(user);
    }

    [HttpPut]
    [Authorize(Roles = "Parent, Child,  Teacher, Admin")]
    public async Task<IActionResult> UpdateLoggedIn([FromHeader(Name = "Authorization")] string token, [FromBody] UserUpdateDTO user)
    {
        var idString = _authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
        }

        var result = await _userService.UpdateUserLoggedInAsync(id, user);
        if (result > 0)
        {
            return Created();
        }
        return NotFound();
    }

    [HttpPut]
    [Authorize(Roles = "Parent, Child,  Teacher, Admin")]
    public async Task<IActionResult> UpdatePassword([FromHeader(Name = "Authorization")] string token, [FromBody] Password password)
    {
        var idString = _authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
        }

        var result = await _userService.UpdateUserPasswordAsync(password.password, id);
        if (result > 0)
        {
            return Created();
        }
        return NotFound();
    }

}
