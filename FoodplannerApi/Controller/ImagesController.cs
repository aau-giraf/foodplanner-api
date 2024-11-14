using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using FoodplannerApi.Helpers;
using System.ComponentModel.DataAnnotations;
using FoodplannerDataAccessSql.Account;
using FoodplannerModels.Account;
using FoodplannerModels.Image;
using FoodplannerServices.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodplannerApi.Controller;

public class ImagesController(IFoodImageService foodImageService) : BaseController
{
    private readonly long _maxFileSize = 2000000000;
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadImage(IFormFile imageFile, int userId)
    {
        if (imageFile.Length == 0) return BadRequest("File is empty");
        if (imageFile.Length >= _maxFileSize) return BadRequest("File too big");
        
        var foodImageId = foodImageService.CreateFoodImage(
            userId,
            imageFile.OpenReadStream(),
            imageFile.FileName,
            imageFile.ContentType,
            imageFile.Length);
        
        return Ok($"FoodImage [{foodImageId}] uploaded successfully");
    }

    [HttpPost]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadImages([Required]IFormFileCollection imageFiles, int userId)
    {
        if (imageFiles.Any(file => file.Length == 0)) return BadRequest("A file is empty");
        if (imageFiles.Any(file => file.Length >= _maxFileSize)) return BadRequest("a file too big");
        var ids = imageFiles
            .Select(async file => await foodImageService.CreateFoodImage(
                userId,
                file.OpenReadStream(),
                file.FileName,
                file.ContentType,
                file.Length))
            .Select(task => task.Result.ToString());
        
        return Ok(ids);
    }


    [HttpDelete]
    [Authorize(Roles = "Child, Parent")]
    [AuthorizeImageOwnerFilter]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteImages(IEnumerable<int> foodImageIds)
    {
        var imageIdList = foodImageIds.ToList();
        if (imageIdList == null || !imageIdList.Any())
            return BadRequest("No imageIds provided");
        
        foodImageIds.ToList().ForEach(id => foodImageService.DeleteImage(id));
        
        return Ok("Images deleted successfully");
    }

    [HttpGet]

    [Authorize(Roles = "Child, Parent")]
    [AuthorizeImageOwnerFilter]
    [ProducesResponseType(typeof(FoodImage), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFoodImage(int foodImageId)
    {
        if (foodImageId < 0)
            return BadRequest("Invalid userId");
        return Ok(foodImageService.GetFoodImage(foodImageId));
    }

    [HttpGet]
    [Authorize(Roles = "Child, Parent, Teacher")]
    [AuthorizeImageOwnerFilter]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPresignedImageLink(int foodImageId)
    {
        var presignedImageLink = await foodImageService.GetFoodImageLink(foodImageId);
        return Ok(presignedImageLink);
    }

    private class AuthorizeImageOwnerFilter : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var authService = context.HttpContext.RequestServices.GetService<AuthService>();
            var userRepository = context.HttpContext.RequestServices.GetService<IUserRepository>();
            var foodImageService = context.HttpContext.RequestServices.GetService<IFoodImageService>();
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var foodImageIds = context.HttpContext.Request.Query["foodImageId"];
            
            if (token == null)
            {
                context.Result = new UnauthorizedResult();
            } else if (foodImageIds.Any(id => id == null))
            {
                context.Result = new BadRequestResult();
            }
            else if (authService == null || foodImageService == null || userRepository == null)
            {
                throw new Exception("Missing services");
            } else
            {
                var userId = int.Parse(authService.RetrieveIdFromJwtToken(token));
                var role = authService.RetrieveRoleFromJwtToken(token);
                if (role == "Teacher")
                {
                    return;
                }
                foreach (var foodImageId in foodImageIds)
                {
                    var foodImage = await foodImageService.GetFoodImage(int.Parse(foodImageId));
                    if (userId == foodImage.UserId) continue;
                    context.Result = new UnauthorizedResult();
                    break;
                }
            }
        }
    }
}