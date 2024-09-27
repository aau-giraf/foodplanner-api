using foodplanner_api.Models;
using foodplanner_api.Service;
using Microsoft.AspNetCore.Mvc;

namespace foodplanner_api.Controller;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase {
    private readonly UserService _userService;

    public UsersController(UserService userService){
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers(){
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsersById(int id){
        var users = await _userService.GetAllUsersByIdAsync(id);
        if (User == null){
            return NotFound();
        }
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user){
        var result = await _userService.CreateUserAsync(user);
        if (result > 0){
            return CreatedAtAction(nameof(GetUsersById), new { id = user.Id }, user);
        }
        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User user){
        if (id != user.Id){
            return BadRequest();
        }
        var result = await _userService.UpdateUserAsync(user);
        if (result > 0){
            return NoContent();
        }
        return NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id){
        var result = await _userService.DeleteUserAsync(id);
        if (result > 0){
            return NoContent();
        }
        return NotFound();
    }

}