using FoodplannerModels.Account;
using FoodplannerServices;
using FoodplannerServices.Account;
using FoodplannerServices.Image;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class ImagesController : BaseController
{
    private readonly ImageService _imagesService;

    public ImagesController(ImageService service)
    {
        _imagesService = service;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile imageFile, int userId)
    {
        if (imageFile is null || imageFile.Length == 0) return BadRequest("No image provided");
        await _imagesService.SaveImage(userId, imageFile.OpenReadStream());
        return Ok("Image uploaded successfully");
    }

    [HttpPost]
    public async Task<IActionResult> UploadImages(IFormFileCollection imageFiles, int userId)
    {
        if (imageFiles is null || imageFiles.Count == 0) return BadRequest("No images provided");
        foreach (IFormFile imageFile in imageFiles)
        {
            if (imageFile is null || imageFile.Length == 0) return BadRequest("empty or null image in collection");
            await _imagesService.SaveImage(userId, imageFile.OpenReadStream());
        }

        return Ok("Images uploaded successfully");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteImage(Guid imageId, int userId)
    {
        if (imageId == Guid.Empty) return BadRequest("No imageId provided");
        await _imagesService.DeleteImage(userId, imageId);
        return Ok("Image deleted successfully");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteImages(IEnumerable<Guid> imageIds, int userId)
    {
        if (userId < 0)
            return BadRequest("Invalid userId provided");

        var imageIdList = imageIds?.ToList();
        if (imageIdList == null || imageIdList.Count == 0)
            return BadRequest("No imageIds provided");
        
        await _imagesService.DeleteImages(userId, imageIdList);
        return Ok("Images deleted successfully");
    }
    

    [HttpGet]
    public async Task<IActionResult> GetImages(int userid, Guid imageId, Stream outStream)
    {
        if (userid <= 0)
            return BadRequest("Invalid userId provided");

        if (imageId == Guid.Empty)
            return BadRequest("No imageId provided");
        
        await _imagesService.LoadImage(userid, imageId, outStream);
        
        return Ok("images loaded successfully");
    }
}