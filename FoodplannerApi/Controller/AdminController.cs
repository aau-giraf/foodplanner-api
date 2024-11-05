using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

[Authorize(Policy = "AdminPolicy")]
public class AdminController : BaseController
{
    private readonly UserService _userService;
    private readonly ChildrenService _childrenService;

    public AdminController(UserService userService, ChildrenService childrenService){
        _userService = userService;
        _childrenService = childrenService;
    }
   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
        var result = await _userService.DeleteUserAsync(id);
        if (result > 0){
            return NoContent();
        }
        return NotFound();
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(){
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id){
        var users = await _userService.GetUserByIdAsync(id);
        if (users == null){
            return NotFound();
        }
        return Ok(users);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] User user){
        if (id != user.Id){
            return BadRequest();
        }
        var result = await _userService.UpdateUserAsync(user);
        if (result > 0){
            return NoContent();
        }
        return NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArchived(int id, [FromBody] UserArchivedDTO userArchivedDTO)
    {
        if (id != userArchivedDTO.id){
            return BadRequest();
        }
        var result = await _userService.UserUpdateArchivedAsync(id, userArchivedDTO.Archived);

        if (result != null){
            return Ok(result);
        }
        
        return NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetNotArchived(){
        var users = await _userService.UserSelectAllNotArchivedAsync();
        return Ok(users);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllChildren(){
        var children = await _childrenService.GetAllChildrenAsync();
        return Ok(children);
    }
    
}