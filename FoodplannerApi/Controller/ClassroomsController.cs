using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;


public class ClassroomsController : BaseController
{
    private readonly IClassroomService _classroomService;
    
    public ClassroomsController(IClassroomService classroomService){
        _classroomService = classroomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(){
        var classroom = await _classroomService.GetAllClassroomAsync();
        return Ok(classroom);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateClassroomDTO createClassroomDTO){
        var result = await _classroomService.InsertClassroomAsync(createClassroomDTO);
        if (result > 0){
             return Created(string.Empty, result);
        }
        return BadRequest();
    }

    
}