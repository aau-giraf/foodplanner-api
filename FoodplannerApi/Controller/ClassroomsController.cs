using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// adds the ClassroomsController to the FoodPlannerApi.controller namespace
namespace FoodplannerApi.Controller;


public class ClassroomsController : BaseController
{

    // Setup of controller with injections to the service layer for classrooms.
    private readonly IClassroomService _classroomService;
    public ClassroomsController(IClassroomService classroomService){
        _classroomService = classroomService;
    }

    // URL: api/Classrooms/GetAll
    // Retrieves all classrooms. Returns 200 OK with the list of classrooms.
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClassroomDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(){
        var classroom = await _classroomService.GetAllClassroomAsync();
        return Ok(classroom);
    }

    // URL: api/Classrooms/Create
    // Creates a new classroom from CreateClassroomDTO. Returns 201 with created classroom ID if successful, or 400 Bad Request if unsuccessful.
    [Authorize(Policy = "AdminPolicy")]
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Create([FromBody] CreateClassroomDTO createClassroomDTO){
        var result = await _classroomService.InsertClassroomAsync(createClassroomDTO);
        if (result > 0){
             return Created(string.Empty, result);
        }
        return BadRequest();
    }

    // URL: api/Classrooms/Update/{id}
    // Updates classroom with given ID using CreateClassroomDTO. Returns 200 OK with updated classroom ID if successful, or 400 Bad Request if unsuccessful.
    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update([FromBody] CreateClassroomDTO createClassroomDTO, int id){
        var result = await _classroomService.UpdateClassroomAsync(createClassroomDTO, id);
        if (result > 0){
            return Ok(result);
        }
        return BadRequest();
    }
    
    // URL: api/Classrooms/Delete/{id}
    // Deletes classroom with given ID. Returns 200 OK if successful, or 400 Bad Request if unsuccessful or if there are children in the classroom.
    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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