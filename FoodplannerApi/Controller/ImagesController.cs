using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FoodplannerDataAccessSql.Image;
using FoodplannerServices.Image;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class ImagesController(IFoodImageService foodImageService) : BaseController
{
    private readonly long _maxFileSize = 2000000000;
    [HttpPost]
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
    public async Task<IActionResult> DeleteImages(IEnumerable<int> foodImageIds)
    {

        var imageIdList = foodImageIds.ToList();
        if (imageIdList == null || !imageIdList.Any())
            return BadRequest("No imageIds provided");
        
        foodImageIds.ToList().ForEach(id => foodImageService.DeleteImage(id));
        
        return Ok("Images deleted successfully");
    }

    [HttpGet]
    public async Task<IActionResult> GetFoodImage(int foodImageId)
    {
        if (foodImageId < 0)
            return BadRequest("Invalid userId provided");
        var foodImage = await foodImageService.GetFoodImage(foodImageId);
        return Ok(new { foodImage });
    }

    [HttpGet]
    public async Task<IActionResult> GetPresignedImageLink(int foodImageId)
    {
        var presignedImageLink = await foodImageService.GetFoodImageLink(foodImageId);
        return Ok(presignedImageLink);
    }
}