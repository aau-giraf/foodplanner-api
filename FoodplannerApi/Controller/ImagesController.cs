using FoodplannerModels.Account;
using FoodplannerServices;
using FoodplannerServices.Account;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class ImagesController : BaseController {
    private readonly ImagesService _imagesService;

    public ImagesController(ImagesService imagesService) {
        _imagesService = imagesService;
    }
    [HttpPost]
    public async Task<IActionResult> UploadImage() {
        // Logic for uploading an image
        return Ok("Image uploaded successfully");
    }

    [HttpGet(user)]
    public async Task<IActionResult> GetAllImagesByUser(User user) {
        var images = await _imagesService.GetAllImagesByUser(user.id);
        if (images == null) {
            return NotFound();
        }

        return Ok(images);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetImagesById(int id) {
        var images = await _imagesService.GetImagesById(id);
        if (id == null) {
            return NotFound();
        }
        return Ok(images);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImageById(int id) {
        if (id == null) {
            return NotFound();
        }
        return Ok(images);
    }

    [HttpUpdate("{id}")]
    public async Task<IActionResult> UpdateImageById(int id) {
        if (id == null) {
            return NotFound();
        }
        return Ok(images);
    }
}