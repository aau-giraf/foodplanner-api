using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Auth;

namespace FoodplannerApi.Controller;

//[Authorize(Policy = "ChildPolicy")]
public class ChildrensController : BaseController
{
    private readonly IChildrenService _childrenService;
    private readonly IAuthService _authService;

    public ChildrensController(IChildrenService childrenService, IAuthService authService)
    {
        _childrenService = childrenService;
        _authService = authService;
    }

    [HttpGet]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(typeof(IEnumerable<ChildrenGetAllDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllChildrenClassesAsync()
    {
        var children = await _childrenService.GetAllChildrenClassesAsync();
        return Ok(children);
    }

    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    [ProducesResponseType(typeof(IEnumerable<Children>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var children = await _childrenService.GetAllChildrenAsync();
        return Ok(children);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetChildrenByParentId([FromHeader(Name = "Authorization")] string token)
    {

        var idString = _authService.RetrieveIdFromJwtToken(token);
        if (!int.TryParse(idString, out int id))
        {
            return BadRequest(new ErrorResponse { Message = ["Error"] });
        }
        var children = await _childrenService.GetChildrenByParentIdAsync(id);
        return Ok(children);
    }

    [Authorize(Policy = "TeacherChildPolicy")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetChildFromChildId(int id)
    {
        var child = await _childrenService.GetChildFromChildIdAsync(id);
        if (child != null)
        {
            return Ok(child);
        }
        return NotFound();
    }

    [HttpGet("{childId}/parents")]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetParentsByChildId(int childId)
    {
        var parents = await _childrenService.GetParentsByChildIdAsync(childId);
        return Ok(parents);
    }

    [HttpPost("{childId}/parents/{userId}")]
    public async Task<IActionResult> AddParentToChild(int childId, int userId)
    {
        var result = await _childrenService.AddParentToChildAsync(userId, childId);
        if (result > 0)
        {
            return Ok(new { Message = "Forældre tilføjet til barn" });
        }
        return BadRequest(new ErrorResponse { Message = new[] { "Kunne ikke tilføje forældre" } });
    }

    [HttpDelete("{childId}/parents/{userId}")]
    public async Task<IActionResult> RemoveParentFromChild(int childId, int userId)
    {
        var result = await _childrenService.RemoveParentFromChildAsync(userId, childId);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound(new ErrorResponse { Message = new[] { "Forældre ikke fundet" } });
    }

    [HttpPost("{childId}/teachers/{userId}")]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddTeacherToChild(int childId, int userId)
    {
        var result = await _childrenService.AddTeacherToChildAsync(userId, childId);
        if (result > 0)
        {
            return Ok(new { Message = "Lærer tilføjet til barn" });
        }
        return BadRequest(new ErrorResponse { Message = new[] { "Kunne ikke tilføje lærer" } });
    }

    [HttpDelete("{childId}/teachers/{userId}")]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTeacherFromChild(int childId, int userId)
    {
        var result = await _childrenService.RemoveTeacherFromChildAsync(userId, childId);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound(new ErrorResponse { Message = new[] { "Lærer ikke fundet" } });
    }

    [HttpGet("{childId}/teachers")]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeachersByChildId(int childId)
    {
        var teachers = await _childrenService.GetTeachersByChildIdAsync(childId);
        return Ok(teachers);
    }

    [HttpGet("by-teacher/{teacherId}")]
    [Authorize(Policy = "TeacherPolicy")]
    [ProducesResponseType(typeof(IEnumerable<Children>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChildrenByTeacherId(int teacherId)
    {
        var children = await _childrenService.GetChildrenByTeacherIdAsync(teacherId);
        return Ok(children);
    }
}