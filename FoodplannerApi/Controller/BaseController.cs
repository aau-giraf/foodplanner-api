using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller
{

    // Route all API requests to this controller, and use the [controller] and [action] placeholders to find the correct file and method.
    [Route("api/[controller]/[action]")]

    // Base controller for inheritance, providing common functionality for all controllers in the application.
    public abstract class BaseController : ControllerBase
    {
        // Possible future common methods or properties for all controllers can be added here.
    }
}