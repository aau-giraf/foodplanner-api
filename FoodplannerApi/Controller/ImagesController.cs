using System.ComponentModel.DataAnnotations;
using FoodplannerModels.Account;
using FoodplannerModels.Image;
using FoodplannerServices.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FoodplannerModels.Auth;

// Adds the ImagesController to the FoodPlannerApi.controller namespace
namespace FoodplannerApi.Controller;

public class ImagesController(IFoodImageService foodImageService, IAuthService authService) : BaseController
{
    
    // Maximum file size for image uploads, set to 2GB
    private readonly long _maxFileSize = 2000000000;

    // URL: api/Images/Upload
    // Uploads multiple images with IFormFileCollection, returns 200 OK with the list of foodImageIds if successful, or 400 Bad Request if unsuccessful.
    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromHeader(Name = "Authorization")] string token, [Required] IFormFileCollection imageFiles)
    {
        try
        {
            // Retrieve UserId from the JWT token
            var idString = authService.RetrieveIdFromJwtToken(token);
            if (!int.TryParse(idString, out int userId))
            {
                return BadRequest(new ErrorResponseDTO {Message = ["Invalid user ID"]});
            }

            // Check that files were provided
            if (imageFiles.Count == 0)
            {
                return BadRequest(new ErrorResponseDTO {Message = ["No files were provided"]});
            }

            // Validate the image files
            if (imageFiles.Any(file => file.Length == 0))
            {
                return BadRequest(new ErrorResponseDTO {Message = ["A file is empty"]});
            }
            if (imageFiles.Any(file => file.Length >= _maxFileSize))
            {
                return BadRequest(new ErrorResponseDTO {Message = ["A file is too big"]});
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
            return BadRequest(new ErrorResponseDTO {Message = [e.Message]});
        }
    }

    // URL: api/Images/Delete
    // Deletes images with given foodImageIds. Returns 200 OK if successful, or 400 Bad Request if unsuccessful.
    [HttpDelete]
    [AuthorizeImageOwnerFilter]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] IEnumerable<int> foodImageIds)
    {
        var imageIdList = foodImageIds.ToList();
        if (imageIdList == null || !imageIdList.Any())
            return BadRequest("No imageIds provided");

        foodImageIds.ToList().ForEach(id => foodImageService.DeleteImage(id));

        return Ok("Images deleted successfully");
    }

    // URL: api/Images/GetFoodImage/{id}
    // Retrieves food image metadata from id. Returns 200 OK with the food image data if found, or 400 Bad Request if the foodImageId is invalid, or 404 Not Found if the food image does not exist.
    [HttpGet("{id}")]
    [AuthorizeImageOwnerFilter]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(FoodImage), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFoodImage(int id)
    {
        if (id < 0)
            return BadRequest("Invalid food image ID");
        var image = await foodImageService.GetFoodImage(id);
        if (image is null) return NotFound();
        return Ok(image);
    }

    // URL: api/Images/GetImageLink/{id}
    // Retrieves presigned image link for a given foodImageId. Returns 200 OK with the presigned link if successful.
    [HttpGet("{id}")]
    [Authorize(Roles = "Parent, Child, Teacher, Admin")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImageLink(int id)
    {
        var presignedImageLink = await foodImageService.GetFoodImageLink(id);
        return Ok(presignedImageLink.Replace("localhost", HttpContext.Request.Host.Host));
    }

    // Custom action filter to authorize image owners
    private class AuthorizeImageOwnerFilter : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {

            // Retrieve services from the request context
            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
            var foodImageService = context.HttpContext.RequestServices.GetService<IFoodImageService>();
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var foodImageIds = context.HttpContext.Request.Query["foodImageId"];

            // Validate the request and services
            if (token == null)
                context.Result = new UnauthorizedResult();
            else if (foodImageIds.Any(id => id == null))
                context.Result = new BadRequestResult();
            else if (authService == null || foodImageService == null)
                throw new Exception("Missing services");
            else
            {
                // Retrieve userId and role from the JWT token
                var userId = int.Parse(authService.RetrieveIdFromJwtToken(token));
                var role = authService.RetrieveRoleFromJwtToken(token);

                // If the user is an teacher, allow access without further checks
                if (role == "Teacher")
                    return;

                // Check if the user is the owner of all the food images
                foreach (var foodImageId in foodImageIds)
                {
                    if (!int.TryParse(foodImageId, out int imageId))
                    {
                        context.Result = new BadRequestResult();
                        return;
                    }

                    var foodImage = await foodImageService.GetFoodImage(imageId);

                    // Check if the image exists
                    if (foodImage == null)
                    {
                        context.Result = new NotFoundResult();
                        return;
                    }

                    // Check if the user owns the image
                    if (userId != foodImage.UserId)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }
        }
    }

    // URL: api/Images/GetAllStockImages
    // Retrieves food stock images. Returns 200 OK with the food image data if found
    [HttpGet]
    [AuthorizeImageOwnerFilter]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(FoodImage), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStockImages()
    {
        var images = await foodImageService.GetAllStockImagesAsync();
        return Ok(images);
    }
}
