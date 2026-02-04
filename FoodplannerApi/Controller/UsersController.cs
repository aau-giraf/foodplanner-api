using FoodplannerModels.Account;
using FoodplannerServices;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using FoodplannerServices.Auth;
using FoodplannerModels.Auth;
using FoodplannerModels.Codes;

namespace FoodplannerApi.Controller;

public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IOneTimePasswordService _oneTimePasswordService;

    public UsersController(IUserService userService, IAuthService authService, IOneTimePasswordService oneTimePasswordService)
    {
        _userService = userService;
        _authService = authService;
        _oneTimePasswordService = oneTimePasswordService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] UserCreateDTO userCreateDto)
    {
        if (Enum.TryParse<UserRole>(userCreateDto.Role, true, out var parsedRole) && parsedRole == UserRole.Child)
        {
            return BadRequest(new ErrorResponse { Message = ["Børn må ikke laves med dette endpoint, istedet skal CreateUserChildren bruges."] });
        }
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var id = await _userService.CreateUserAsync(userCreateDto);
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
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> CreateUserChildren([FromHeader(Name = "Authorization")] string token, [FromBody] UserCreateChildDTO userCreateChildDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var idString = _authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int parentId))
            {
                return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
            }
            
            var id = await _userService.CreateChildrenUserAsync(userCreateChildDto, parentId);
         
            if (id > 0)
            {
                return Created(string.Empty, id);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Email = new[] { e.Message } });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _userService.GetJWTByEmailAndPasswordAsync(login.Email, login.Password);
            
            //TODO: Handle usecase for parent using one time password
            /*if (!string.IsNullOrEmpty(user.Code) && result != null)
            {
                if (Enum.TryParse<UserRole>(result.Role, true, out var parsedRole) && parsedRole == UserRole.Child)
                {
                    return BadRequest(new ErrorResponse { Message = ["Børn må ikke laves med dette endpoint, istedet skal LoginChild bruges."] });
                }
                var code = await _oneTimePasswordService.GetOneTimePassword(user.Code);
                code.Used = true;
                code.UsedByUser = int.Parse(_authService.RetrieveIdFromJwtTokenNoBearer(result.JWT));

                if (await _oneTimePasswordService.UpdateOneTimePassword(code) == 0)
                    return BadRequest(new ErrorResponse { Message = ["Fejlede i at opdatere engangskode"] });
                if (await _oneTimePasswordService.RedeemOneTimePassword(code.Code) == 0)
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

    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LoginChild([FromBody] LoginChild user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var result = await _userService.GetJWTByEmailAsync(user.Email);
            if (!string.IsNullOrEmpty(user.Code) && result != null)
            {
                int id = int.Parse(_authService.RetrieveIdFromJwtTokenNoBearer(result.JWT));
                if (await _oneTimePasswordService.RedeemOneTimePassword(user.Code, id) == 0)
                    return BadRequest("Failed while trying to redeem the one time code");
            }

            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new ErrorResponse { Message = ["Email eller password er forkert"] });
        }
        catch
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
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EmailExists([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new ErrorResponse { Message = ["Email skal angives"] });
        }

        var exists = await _userService.UserEmailExistsAsync(email);
        return Ok(new { EmailExists = exists });
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
    public async Task<IActionResult> UpdateLoggedIn([FromHeader(Name = "Authorization")] string token, [FromBody] UserUpdateLoggedInDTO user)
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

    [HttpDelete]
    [Authorize(Roles = "Parent, Child, Teacher, Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLoggedInUser([FromHeader(Name = "Authorization")] string token)
    {
        var idString = _authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
        }

        int result = await _userService.DeleteUserAsync(id);

        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }

}
