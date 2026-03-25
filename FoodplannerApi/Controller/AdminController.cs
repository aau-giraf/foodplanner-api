using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

[Authorize(Policy = "AdminPolicy")]
public class AdminController : BaseController
{
    private readonly IUserService _userService;
    private readonly IChildrenService _childrenService;

    public AdminController(IUserService userService, IChildrenService childrenService)
    {
        _userService = userService;
        _childrenService = childrenService;
    }

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
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

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

    [HttpPut("{id}")]
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

    [HttpPut("updaterole/{id}")]
    public async Task<IActionResult> UpdateRoleApproved(int id, [FromBody] UserRoleDTO userRoleDTO)
    {
        var result = await _userService.UserUpdateRoleApprovedAsync(id, userRoleDTO.role_approved);

        if (result)
        {
            return Ok(result);
        }

        return NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetNotArchived()
    {
        var users = await _userService.UserSelectAllNotArchivedAsync();
        return Ok(users);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllChildren()
    {
        var children = await _childrenService.GetAllChildrenAsync();
        return Ok(children);
    }

    [HttpPut]
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