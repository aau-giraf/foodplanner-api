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

        public OneTimePasswordController(IOneTimePasswordService passwordService, IAuthService authService)
        {
            _passwordService = passwordService;
            _authService = authService;
        }

        [HttpPost]
        [Authorize(Roles = "Parent")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                var idString = _authService.RetrieveIdFromJwtToken(token);
                if (!int.TryParse(idString, out int id))
                {
                    return BadRequest(new ErrorResponse { Message = ["Id er ikke et tal"] });
                }
                var result = await _passwordService.CreateOneTimePassword(id);

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
    }
}
