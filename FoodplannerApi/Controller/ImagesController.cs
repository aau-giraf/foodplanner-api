using System.ComponentModel.DataAnnotations;
using FoodplannerModels.Account;
using FoodplannerServices.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FoodplannerServices.Auth;
using FoodplannerModels.Auth;

// Adds the ImagesController to the FoodPlannerApi.controller namespace
namespace FoodplannerApi.Controller;

public class ImagesController(IFoodImageService foodImageService, IAuthService authService) : BaseController
{
    
    // Maximum file size for image uploads, set to 2GB
    private readonly long _maxFileSize = 2000000000;

    // URL: api/Images/UploadImage
    // Uploads image file with IFormFile, returns 200 OK with the foodImageId if successful, or 400 Bad Request if unsuccessful.
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage([FromHeader(Name = "Authorization")] string token, IFormFile imageFile)
    {
        try
        {
            // Retrieve UserId from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponse { Message = ["Invalid user ID"] });
            }
            
            // Check if file is empty or exceeds the maximum file size
            if (imageFile.Length == 0)
            {
                return BadRequest(new ErrorResponse { Message = ["File is empty"] });
            }
            if (imageFile.Length >= _maxFileSize)
            {
                return BadRequest(new ErrorResponse { Message = ["File is too big"] });
            }

            // Validate the image file
            var foodImageId = await foodImageService.CreateFoodImage(
                userId,
                imageFile.OpenReadStream(),
                imageFile.FileName,
                imageFile.ContentType,
                imageFile.Length
            );

            return Ok(foodImageId);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse { Message = [e.Message] });
        }
    }

    // URL: api/Images/UploadImages
    // Uploads multiple images with IFormFileCollection, returns 200 OK with the list of foodImageIds if successful, or 400 Bad Request if unsuccessful.
    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImages([FromHeader(Name = "Authorization")] string token, [Required] IFormFileCollection imageFiles)
    {
        try
        {
            // Retrieve UserId from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponse {Message = ["Invalid user ID"]});
            }

            // Check that files were provided
            if (imageFiles.Count == 0)
            {
                return BadRequest(new ErrorResponse {Message = ["No files were provided"]});
            }

            // Validate the image files
            if (imageFiles.Any(file => file.Length == 0))
            {
                return BadRequest(new ErrorResponse {Message = ["A file is empty"]});
            }
            if (imageFiles.Any(file => file.Length >= _maxFileSize))
            {
                return BadRequest(new ErrorResponse {Message = ["A file is too big"]});
            }

            // Create all food images
            var foodImageIds = await Task.WhenAll(
                imageFiles.Select(file =>
                    foodImageService.CreateFoodImage(
                        userId,
                        file.OpenReadStream(),
                        file.FileName,
                        file.ContentType,
                        file.Length))
            );

            return Ok(foodImageIds);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new ErrorResponse {Message = [e.Message]});
        }
    }

    // URL: api/Images/DeleteImages
    // Deletes images with given foodImageIds. Returns 200 OK if successful, or 400 Bad Request if unsuccessful.
    [HttpDelete]
    [AuthorizeImageOwnerFilter]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteImages(IEnumerable<int> foodImageIds)
    {
        var imageIdList = foodImageIds.ToList();
        if (imageIdList == null || !imageIdList.Any())
            return BadRequest("No imageIds provided");

        foodImageIds.ToList().ForEach(id => foodImageService.DeleteImage(id));

        return Ok("Images deleted successfully");
    }

    // URL: api/Images/GetFoodImage
    // Retrieves food image from id. Returns 200 OK with the food image data if found, or 400 Bad Request if the foodImageId is invalid, or 404 Not Found if the food image does not exist.
    [HttpGet]
    [AuthorizeImageOwnerFilter]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(FoodImageDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFoodImage(int foodImageId)
    {
        if (foodImageId < 0)
            return BadRequest("Invalid food image ID");
        var image = await foodImageService.GetFoodImage(foodImageId);
        if (image is null) return NotFound();
        return Ok(image);
    }

    // URL: api/Images/GetPresignedImageLink
    // Retrieves presigned image link for a given foodImageId. Returns 200 OK with the presigned link if successful.
    [HttpGet]
    [Authorize(Roles = "Parent, Child, Teacher, Admin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPresignedImageLink(int foodImageId)
    {
        var presignedImageLink = await foodImageService.GetFoodImageLink(foodImageId);
        return Ok(presignedImageLink.Replace("localhost", HttpContext.Request.Host.Host));
    }

    // Custom action filter to authorize image owners
    private class AuthorizeImageOwnerFilter : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {

            // Retrieve services from the request context
            var authService = context.HttpContext.RequestServices.GetService<AuthService>();
            var foodImageService = context.HttpContext.RequestServices.GetService<IFoodImageService>();
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var foodImageIds = context.HttpContext.Request.Query["foodImageId"];

            // Validate the request and services
            if (token == null)
            {
                context.Result = new UnauthorizedResult();
            }
            else if (foodImageIds.Any(id => id == null))
            {
                context.Result = new BadRequestResult();
            }
            else if (authService == null || foodImageService == null)
            {
                throw new Exception("Missing services");
            }
            else
            {

                // Retrieve userId and role from the JWT token
                var userId = int.Parse(authService.RetrieveIdFromJwtToken(token));
                var role = authService.RetrieveRoleFromJwtToken(token);

                // If the user is an teacher, allow access without further checks
                if (role == "Teacher")
                {
                    return;
                }

                // Check if the user is the owner of all the food images
                foreach (var foodImageId in foodImageIds)
                {
                    if (foodImageId == null)
                    {
                        continue;
                    }
                    var foodImage = await foodImageService.GetFoodImage(int.Parse(foodImageId));
                    if (userId == foodImage.UserId) continue;
                    context.Result = new UnauthorizedResult();
                    break;
                }
            }
        }
    }
}