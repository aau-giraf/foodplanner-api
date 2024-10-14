using FoodplannerModels.Account;
using FoodplannerServices;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

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
        if (!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var id = await _userService.CreateUserAsync(user);
        if (id == -1){
            return BadRequest("Email eksistere allerede");
        }
        if (id > 0){
            user.Id = id;
            return CreatedAtAction(nameof(Get), new { id = id}, user);
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

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] Login user){
        if (!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        var result = await _userService.GetUserByEmailAndPasswordAsync(user.Email, user.Password);
        if (result != null){
            return Ok(result);
        }
        return NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> RoleRequests(){
        var users = await _userService.GetAllUsersNotApprovedAsync();
        return Ok(users);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> ApproveRole(int id){
        var result = await _userService.ApproveUserRoleAsync(id);
        if (result > 0){
            return NoContent();
        }
        return NotFound();
    }
}