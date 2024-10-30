using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

[Authorize(Policy = "AdminPolicy")]
public class AdminController : BaseController
{
    private readonly UserService _userService;
    
    public AdminController(UserService userService){
        _userService = userService;
    }
   
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id){
        var result = await _userService.DeleteUserAsync(id);
        if (result > 0){
            return NoContent();
        }
        return NotFound();
    }
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(){
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id){
        var users = await _userService.GetUserByIdAsync(id);
        if (users == null){
            return NotFound();
        }
        return Ok(users);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

}