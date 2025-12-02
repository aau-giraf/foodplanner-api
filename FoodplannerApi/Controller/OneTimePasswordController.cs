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
        private readonly IChildrenService _childrenService;

        public OneTimePasswordController(IOneTimePasswordService passwordService, IAuthService authService, IUserService userService, IChildrenService childrenService)
        {
            _passwordService = passwordService;
            _authService = authService;
            _userService = userService;
            _childrenService = childrenService;
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

                // If a childUser is specified, check if the parent has a relation to this child
                if (childUser != null)
                {
                    var children = await _childrenService.GetChildrenByParentIdAsync(id);
                    if (!children.Any(c => c.ChildId == childUser.Value))
                    {
                        return BadRequest("You do not have a relation to this child.");
                    }
                }

                int result = await _passwordService.CreateOneTimePassword(id, childUser);

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
                    return BadRequest("Id is not a number");

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
