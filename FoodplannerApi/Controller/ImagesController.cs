using FoodplannerDataAccessSql.Image;
using FoodplannerModels.Account;
using FoodplannerServices;
using FoodplannerServices.Account;
using FoodplannerServices.Image;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class ImagesController(IImageService imageService, IFoodImageRepository foodImageRepository) : BaseController
{
    private readonly long _maxFileSize = 2000000000;
    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile imageFile, int userId)
    {
        if (imageFile.Length == 0) return BadRequest("File is empty");
        if (imageFile.Length >= _maxFileSize) return BadRequest("File too big");
        var imageId = await imageService.SaveImageAsync(
            userId, imageFile.OpenReadStream(), imageFile.ContentType);
        var foodImageId = await foodImageRepository.InsertImageAsync(
            imageId.ToString(), 
            userId, 
            imageFile.FileName,
            imageFile.ContentType, 
            imageFile.Length);
        return Ok($"FoodImage [{foodImageId}] uploaded successfully");
    }

    [HttpPost]
    public async Task<IActionResult> UploadImages(IFormFileCollection imageFiles, int userId)
    {
        var ids = imageFiles
            .Select(async file => await imageService.SaveImageAsync(userId, file.OpenReadStream(), file.ContentType))
            .Select(task => task.Result.ToString());
        
        return Ok(ids);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteImage(int foodImageId, int userId)
    {
        var foodImage = await foodImageRepository.GetImageByIdAsync(foodImageId);
        await imageService.DeleteImageAsync(userId, Guid.Parse(foodImage.ImageId), foodImage.ImageFileType);
        return Ok("Image deleted successfully");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteImages(IEnumerable<Guid> imageIds, int userId)
    {
        if (userId < 0)
            return BadRequest("Invalid userId provided");

        var imageIdList = imageIds?.ToList();
        if (imageIdList == null || !imageIdList.Any())
            return BadRequest("No imageIds provided");
        
        await imageService.DeleteImagesAsync(userId, imageIdList);
        return Ok("Images deleted successfully");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFoodImage(int foodImageId)
    {
        if (foodImageId < 0)
            return BadRequest("Invalid userId provided");
        
        var foodImage = await foodImageRepository.GetImageByIdAsync(foodImageId);
        return Ok(new { foodImage });
    }

    [HttpGet]
    public async Task<IActionResult> GetPresignedImageLink(int foodImageId)
    {
        var foodImage = await foodImageRepository.GetImageByIdAsync(foodImageId);
        var presignedImageLink = await imageService.LoadImagePresignedAsync(foodImage.UserId, Guid.Parse(foodImage.ImageId), foodImage.ImageFileType);
        if (presignedImageLink == null)
        {
            return NotFound("Image does not exist");
        }

        return Ok(presignedImageLink);
    }
}