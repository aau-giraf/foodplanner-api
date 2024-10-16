using Microsoft.AspNetCore.Authorization;

namespace FoodplannerApi.Controller;

[Authorize(Policy = "TeacherPolicy")]
public class TeachersController : BaseController
{
    
}