using FoodplannerApi.Helpers;
using FoodplannerModels.Account;
using FoodplannerServices;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class UsersController : BaseController {
    private readonly UserService _userService;
    private readonly AuthService _authService;

    public UsersController(UserService userService,AuthService authService){
        _userService = userService;
        _authService = authService;
    }
    [HttpGet]
    public async Task<IActionResult> GetBearerTest()
    {
        //Generates a token for development purposes, Status must be Active.
        //Roles can be: Admin, Child, Teacher, Parent
        var user = new User
        {
            Id = 27,
            FirstName = "test",
            LastName = "test",
            Email = "user@test.com",
            Password = "test",
            Role = "admin",
            RoleApproved = true
        };

        var token = _authService.GenerateJWTToken(user);
        
        return Ok(token);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserCreateDTO userCreate){
        if (!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        try{
            var id = await _userService.CreateUserAsync(userCreate);
            if (id > 0){
                return Ok(id);
            }
            return BadRequest();
        } catch (InvalidOperationException e){
            return BadRequest(new {EMAIL = e.Message});
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] Login user){
        if (!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        try{
            var result = await _userService.GetJWTByEmailAndPasswordAsync(user.Email, user.Password);
            if (result != null){
                return Ok(result);
            }
            return BadRequest(new {message = "Email eller password er forkert"});
        } catch (InvalidOperationException e){
            return BadRequest(new {message = "Email eller password er forkert"});
        }
    }
}