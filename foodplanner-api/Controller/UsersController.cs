using foodplanner_models.Account;
using foodplanner_services;
using Microsoft.AspNetCore.Mvc;

namespace foodplanner_api.Controller;

[ApiController]
public class UsersController : BaseController {
    private readonly UserService _userService;

    public UsersController(UserService userService){
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(){
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id){
        var users = await _userService.GetUserByIdAsync(id);
        if (User == null){
            return NotFound();
        }
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user){
        var result = await _userService.CreateUserAsync(user);
        if (result > 0){
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }
        return BadRequest();
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
        var result = await _userService.DeleteUserAsync(id);
        if (result > 0){
            return NoContent();
        }
        return NotFound();
    }

}