using FoodplannerModels.Account;
using FoodplannerServices;
using FoodplannerServices.Account;
using FoodplannerServices.Image;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class ImagesController(IImageService imageService) : BaseController
{
    private readonly long _maxFileSize = 2000000000;
    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile imageFile, int userId)
    {
        if (imageFile.Length == 0) return BadRequest("File is empty");
        if (imageFile.Length >= _maxFileSize) return BadRequest("File too big");
        await imageService.SaveImageAsync(userId, imageFile.OpenReadStream());
        return Ok("Image uploaded successfully");
    }

    [HttpPost]
    public async Task<IActionResult> UploadImages(IFormFileCollection imageFiles, int userId)
    {
        var ids = imageFiles
            .Select(async file => await imageService.SaveImageAsync(userId, file.OpenReadStream()))
            .Select(task => task.Result.ToString());
        
        return Ok(ids);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteImage(Guid imageId, int userId)
    {
        if (imageId == Guid.Empty) return BadRequest("No imageId provided");
        await imageService.DeleteImageAsync(userId, imageId);
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
    public async Task<IActionResult> GetImage(int userid, Guid imageId)
    {
        if (userid < 0)
            return BadRequest("Invalid userId provided");

        if (imageId == Guid.Empty)
            return BadRequest("No imageId provided");

        var outStream = new MemoryStream();
        await imageService.LoadImageAsync(userid, imageId, outStream);
        string base64 = Convert.ToBase64String(outStream.ToArray());
        
        return Ok(base64);
    }
}