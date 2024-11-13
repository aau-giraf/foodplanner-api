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
    public async Task<IActionResult> GetAllChildrenClassesAsync(){
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



    
    [HttpPost]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] ChildrenCreateDTO childrenCreate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var idString = _authService.RetrieveIdFromJWTToken(token); // Use the method to get the parentId from the token    
            if (!int.TryParse(idString, out int parentId))
            {
                return BadRequest(new ErrorResponse { Message = new[] { "Id er ikke et tal" } });
            }

            var childToCreate = new ChildrenCreateParentDTO
            {
                FirstName = childrenCreate.FirstName,
                LastName = childrenCreate.LastName,
                parentId = parentId,
                classId = childrenCreate.classId
            };

            var id = await _childrenService.CreateChildrenAsync(childToCreate);
            if (id > 0)
            {
                return Created(string.Empty, id);
            }
            return BadRequest();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _childrenService.DeleteChildrenAsync(id);
        if (result > 0)
        {
            return NoContent();
        }
        return NotFound();
    }
}