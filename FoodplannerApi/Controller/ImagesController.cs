using FoodplannerModels.Images;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class ImagesController(IImageService imageService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile imageFile)
    {
        if (imageFile is null) return BadRequest();
        await imageService.SaveImage(0, imageFile.OpenReadStream());
        
        return Ok("Uploaded image");
    }
    
    [HttpPost]
    public async Task<IActionResult> UploadImages(IEnumerable<IFormFile> imageFiles)
    {
        var formFiles = imageFiles.ToList();
        if (!formFiles.Any()) return BadRequest();
        var results = await Task.WhenAll(formFiles.Select(file => imageService.SaveImage(0, file.OpenReadStream())));
        return Ok($"Uploaded image: {results}");
    }
}