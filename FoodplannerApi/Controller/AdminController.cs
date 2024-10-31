using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

[Authorize(Policy = "AdminPolicy")]
public class AdminController : BaseController
{
    private readonly UserService _userService;
    private readonly ClassroomService _classroomService;

    public AdminController(UserService userService, ClassroomService classroomService){
        _userService = userService;
        _classroomService = classroomService;
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

    /* [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateClassroomDTO createClassroomDTO){
        var result = await _classroomService.InsertClassroomAsync(createClassroomDTO);
        if (result > 0){
             return Created(string.Empty, result);
        }
        return BadRequest();
    } */


}