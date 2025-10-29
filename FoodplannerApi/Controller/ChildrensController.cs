using FoodplannerModels.Account;
using FoodplannerApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

//[Authorize(Policy = "ChildPolicy")]
public class ChildrensController : BaseController
{
    private readonly IChildrenService _childrenService;
    private readonly AuthService _authService;

    public ChildrensController(IChildrenService childrenService, AuthService authService)
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

    [Authorize(Policy = "TeacherPolicy")]
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
}