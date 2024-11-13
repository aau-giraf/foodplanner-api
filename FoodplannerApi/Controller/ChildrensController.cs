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
    
    public ChildrensController(IChildrenService childrenService, AuthService authService){
        _childrenService = childrenService;
                _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] ChildrenCreateDTO childrenCreate){
        if (!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        try{
            var idString = _authService.RetrieveIdFromJwtToken(token); // Use the method to get the parentId from the token    
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
            if (id > 0){
                return Created(string.Empty, id);
            }
            return BadRequest();
        } catch (InvalidOperationException e){
            return BadRequest(e.Message);
        }
    }

}