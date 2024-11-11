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

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateClassroomDTO createClassroomDTO){
        var result = await _classroomService.InsertClassroomAsync(createClassroomDTO);
        if (result > 0){
             return Created(string.Empty, result);
        }
        return BadRequest();
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromBody] CreateClassroomDTO createClassroomDTO, int id){
        var result = await _classroomService.UpdateClassroomAsync(createClassroomDTO, id);
        if (result > 0){
            return Ok(result);
        }
        return BadRequest();
    }
    
    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id){
        var isChildrenInClassroom = await _classroomService.CheckChildrenInClassroom(id);
        if (isChildrenInClassroom){
            return BadRequest(new ErrorResponse{Message = ["Der er børn i klassen"]});
        }
        var result = await _classroomService.DeleteClassroomAsync(id);
        if (result > 0){
            return Ok(result);
        }
        return BadRequest();
    }
}