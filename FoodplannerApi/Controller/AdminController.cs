using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Adds the Admincontroller to the FoodPlannerApi.
namespace FoodplannerApi.Controller;

[Authorize(Policy = "AdminPolicy")]
public class AdminController : BaseController
{

    // Setup of controller with injections to the service layer for user and children.
    private readonly IUserService _userService;
    private readonly IChildrenService _childrenService;
    public AdminController(IUserService userService, IChildrenService childrenService)
    {
        _userService = userService;
        _childrenService = childrenService;
    }

    // URL: api/Admin/Delete/{id}
    // Deletes a user by their ID. Returns 204 No Content if successful, or 404 Not Found if the user does not exist.
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }

    // URL: api/Admin/GetAll
    // Retrieves all users. Returns 200 OK with the list of users.
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // URL: api/Admin/Get/{id}
    // Retrieves a user by their ID. Returns 200 OK with the user data if found, or 404 Not Found if the user does not exist.
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var users = await _userService.GetUserByIdAsync(id);
        if (users == null)
        {
            return NotFound();
        }
        return Ok(users);
    }

    // URL: api/Admin/Update/UpdateUser/{id}
    // Updates a users of a user by their ID. Returns either 400 Bad Request if the input is invalid, 204 No Content if the update is successful, or 404 Not Found if the user does not exist.
    [HttpPut("updateuser/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDTO user)
    {
        var result = await _userService.UpdateUserAsync(user, id);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }

    // URL: api/Admin/UpdateArchived/{id}
    // Updates the archived status of a user by their ID. Returns 200 OK if successful, or 404 Not Found if the user does not exist.
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateArchived(int id)
    {

        var result = await _userService.UserUpdateArchivedAsync(id);

        if (result)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    // URL: api/Admin/UpdateRole/{id}
    // Updates the role of a user by their ID. Returns 200 OK if successful, or 404 Not Found if the user does not exist.
    [HttpPut("updaterole/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoleApproved(int id, [FromBody] UserRoleDTO userRoleDTO)
    {
        var result = await _userService.UserUpdateRoleApprovedAsync(id, userRoleDTO.role_approved);

        if (result)
        {
            return Ok(result);
        }

        return NotFound();
    }
    
    // URL: api/Admin/GetNotArchived
    // Retrieves all users that are not archived. Returns 200 OK with the list of users
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotArchived()
    {
        var users = await _userService.UserSelectAllNotArchivedAsync();
        return Ok(users);
    }

    // URL: api/Admin/GetAllChildren
    // Retrieves all children. Returns 200 OK with the list of children.
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChildrenDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllChildren()
    {
        var children = await _childrenService.GetAllChildrenAsync();
        return Ok(children);
    }

    // URL: api/Admin/UpdateChild
    // Updates a child with ChildrenDTO model. Returns 204 No Content if successful, or 404 Not Found if the child does not exist.
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateChild([FromBody] ChildrenDTO childrenDto)
    {
        var result = await _childrenService.UpdateChildrenAsync(childrenDto);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }
}