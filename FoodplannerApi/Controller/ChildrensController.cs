using Microsoft.AspNetCore.Authorization;

namespace FoodplannerApi.Controller;

[Authorize(Policy = "ChildPolicy")]
public class ChildrensController : BaseController
{
    
}