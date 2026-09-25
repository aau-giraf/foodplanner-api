using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerModels.Codes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Adds the OneTimePasswordController to the FoodPlannerApi.Controller namespace
namespace FoodplannerApi.Controller;

public class OneTimePasswordController(IOneTimePasswordService passwordService, IAuthService authService, IChildrenService childrenService) : BaseController
{

    // URL: api/OneTimePassword/Create
    // Creates a one-time password for the authenticated parent user, optionally for a specified child user
    // should maybe change to /api/OneTimePassword/Create/{childId}
    [HttpPost]
    [Authorize(Roles = "Parent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, int? childUser)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // If a childUser is specified, check if the parent has a relation to this child
            if (childUser != null)
            {
                var children = await childrenService.GetChildrenByParentIdAsync(id);
                if (!children.Any(c => c.ChildId == childUser.Value))
                {
                    return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
                }
            }

            // Create a one-time password for the parent or specified child user
            int result = await passwordService.CreateOneTimePassword(id, childUser);
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

    // URL: api/OneTimePassword/Redeem
    // Redeems a one-time password for the authenticated user
    // Should maybe change to /api/OneTimePassword/Redeem/{code}
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Redeem([FromHeader(Name = "Authorization")] string token, string code)
    {
        try
        {
            // Retrieve the user ID from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int id))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }

            // Redeem the one-time password for the user
            var userRole = authService.RetrieveRoleFromJwtToken(token);
            var result = await passwordService.RedeemOneTimePassword(code, id);
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

