using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerModels.Codes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller
{
    public class OneTimePasswordController : BaseController
    {
        private readonly IOneTimePasswordService _passwordService;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public OneTimePasswordController(IOneTimePasswordService passwordService, IAuthService authService, IUserService userService)
        {
            _passwordService = passwordService;
            _authService = authService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "Parent")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, int? childUser)
        {
            try
            {
                var idString = _authService.RetrieveIdFromJwtToken(token);
                if (!int.TryParse(idString, out int id))
                {
                    return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
                }
                int result;
                if(childUser != null)
                {
                    result = await _passwordService.CreateOneTimePassword(id, childUser);
                }
                else
                {
                    result = await _passwordService.CreateOneTimePassword(id, null);
                }

                if (result > 0)
                {
                    return Created(string.Empty, result);
                }
                return NotFound();
            }
            catch
            {
                return BadRequest("Most likely not logged in or a parent");
            }
            
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Redeem([FromHeader(Name = "Authorization")] string token, string code)
        {
            try
            {
                var idString = _authService.RetrieveIdFromJwtToken(token);
                if (!int.TryParse(idString, out int id))
                    return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });

                var otp = await _passwordService.GetOneTimePassword(code);
                if (otp == null)
                    return NotFound("Code not found");

                var userRole = _authService.RetrieveRoleFromJwtToken(token);

                if (otp.ChildUser == null)
                    return BadRequest("This code is meant for logging in a child user.");

                otp.UsedByUser = id;
                await _passwordService.UpdateOneTimePassword(otp);

                var result = await _passwordService.RedeemOneTimePassword(code);

                if (result > 0)
                    return Ok("Code redeemed successfully.");

                return BadRequest("Could not redeem code.");
            }
            catch
            {
                return BadRequest("Most likely not logged in or not a parent");
            }
        }
    }
}
