using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller
{
    [Route("api/[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        public string Language
        {
            get
            {
                var lang = Request.Headers["Accept-Language"].ToString();
                return string.IsNullOrEmpty(lang) ? "*" : lang;
            }
        }

        protected StatusCodeResult NotAllowed()
        {
            return StatusCode(403);
        }

        protected ObjectResult NotAllowed(object value)
        {
            return StatusCode(403, value);
        }

        protected StatusCodeResult NotAuthorized()
        {
            return StatusCode(401);
        }
        
        protected ObjectResult NotAuthorized(object value)
        {
            return StatusCode(401, value);
        }

        protected string GetControllerName()
        {
            return $"{{\"Controller:\":\"{this.GetType().FullName}\"}}";
        }
    }
}
