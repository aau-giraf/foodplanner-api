using Microsoft.AspNetCore.Authorization;

namespace FoodplannerApi.Controller;

[Authorize(Policy = "ParentPolicy")]
public class ParentsController : BaseController
{
    
}