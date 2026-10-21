using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Auth;

// Adds the ChildrensController to the FoodPlannerApi.controller namespace
namespace FoodplannerApi.Controller;

public class ChildrensController(IChildrenService childrenService, IAuthService authService) : BaseController
{

    // URL: api/Childrens/GetAllChildrenClassesAsync
    // Retrieves all children classes. Returns 200 OK with the list of children classes.
    [HttpGet]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(typeof(IEnumerable<ChildrenGetAllDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllChildrenClassesAsync()
    {
        var children = await childrenService.GetAllChildrenClassesAsync();
        return Ok(children);
    }

    // URL: api/Childrens/GetAll
    // Retrieves all children. Returns 200 OK with the list of children.
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<Children>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var children = await childrenService.GetAllChildrenAsync();
        return Ok(children);
    }
    
    // URL: api/Childrens/GetChildrenByParentId
    // Retrieves all children associated with a parent ID extracted from the JWT token in the Authorization header. Returns 200 OK with the list of children, or 400 Bad Request if the ID cannot be parsed.
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Children>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetChildrenByParentId([FromHeader(Name = "Authorization")] string token)
    {

        var idString = authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Error"] });
        }
        var children = await childrenService.GetChildrenByParentIdAsync(id);
        return Ok(children);
    }

    // URL: api/Childrens/GetChildFromChildId/{id}
    // Retrieves a child by their ID. Returns 200 OK with the child data if found, or 404 Not Found if the child does not exist.
    [Authorize(Policy = "TeacherChildPolicy")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetChildFromChildId(int id)
    {
        var child = await childrenService.GetChildFromChildIdAsync(id);
        if (child != null)
        {
            return Ok(child);
        }
        return NotFound();
    }

    // URL: api/Childrens/GetParentsByChildId/{childId}/parents
    // Retrieves all parents associated with a child ID. Returns 200 OK with the list of parents.
    [HttpGet("{childId}/parents")]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetParentsByChildId(int childId)
    {
        var parents = await childrenService.GetParentsByChildIdAsync(childId);
        return Ok(parents);
    }

    // URL: api/Childrens/AddParentToChild/{childId}/parents/{userId}
    // Adds a parent to a child by their IDs. Returns 200 OK if successful, or 400 Bad Request if the operation fails.
    [HttpPost("{childId}/parents/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddParentToChild(int childId, int userId)
    {
        var result = await childrenService.AddParentToChildAsync(userId, childId);
        if (result > 0)
        {
            return Ok(new { Message = "Forældre tilføjet til barn" });
        }
        return BadRequest(new ErrorResponse { Message = new[] { "Kunne ikke tilføje forældre" } });
    }

    // URL: api/Childrens/RemoveParentFromChild/{childId}/parents/{userId}
    // Removes a parent from a child by their IDs. Returns 204 No Content if successful, or 404 Not Found if the parent does not exist for the child.
    [HttpDelete("{childId}/parents/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveParentFromChild(int childId, int userId)
    {
        var result = await childrenService.RemoveParentFromChildAsync(userId, childId);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound(new ErrorResponse { Message = new[] { "Forældre ikke fundet" } });
    }

    // URL: api/Childrens/AddTeacherToChild/{childId}/teachers/{userId}
    // Adds a teacher to a child by their IDs. Returns 200 OK if successful, or 400 Bad Request if the operation fails.
    [HttpPost("{childId}/teachers/{userId}")]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddTeacherToChild(int childId, int userId)
    {
        var result = await childrenService.AddTeacherToChildAsync(userId, childId);
        if (result > 0)
        {
            return Ok(new { Message = "Lærer tilføjet til barn" });
        }
        return BadRequest(new ErrorResponse { Message = new[] { "Kunne ikke tilføje lærer" } });
    }

    // URL: api/Childrens/RemoveTeacherFromChild/{childId}/teachers/{userId}
    // Removes a teacher from a child by their IDs. Returns 204 No Content if successful, or 404 Not Found if the teacher does not exist for the child.
    [HttpDelete("{childId}/teachers/{userId}")]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTeacherFromChild(int childId, int userId)
    {
        var result = await childrenService.RemoveTeacherFromChildAsync(userId, childId);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound(new ErrorResponse { Message = new[] { "Lærer ikke fundet" } });
    }

    // URL: api/Childrens/GetTeachersByChildId/{childId}/teachers
    // Retrieves all teachers associated with a child ID. Returns 200 OK with the list of teachers.
    [HttpGet("{childId}/teachers")]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeachersByChildId(int childId)
    {
        var teachers = await childrenService.GetTeachersByChildIdAsync(childId);
        return Ok(teachers);
    }

    // URL: api/Childrens/GetChildrenByTeacherId/by-teacher/{teacherId}
    // Retrieves all children associated with a teacher ID. Returns 200 OK with the list of children.
    [HttpGet("by-teacher/{teacherId}")]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(typeof(IEnumerable<Children>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChildrenByTeacherId(int teacherId)
    {
        var children = await childrenService.GetChildrenByTeacherIdAsync(teacherId);
        return Ok(children);
    }
}