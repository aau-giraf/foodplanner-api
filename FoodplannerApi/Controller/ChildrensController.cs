using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

//[Authorize(Policy = "ChildPolicy")]
public class ChildrensController : BaseController
{
    private readonly IChildrenService _childrenService;
    
    public ChildrensController(IChildrenService childrenService){
        _childrenService = childrenService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(){
        var children = await _childrenService.GetAllChildrenAsync();
        return Ok(children);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ChildrenCreateDTO childrenCreate){
        if (!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        try{
            var id = await _childrenService.CreateChildrenAsync(childrenCreate);
            if (id > 0){
                return Created(string.Empty, id);
            }
            return BadRequest();
        } catch (InvalidOperationException e){
            return BadRequest(e.Message);
        }
    }

}