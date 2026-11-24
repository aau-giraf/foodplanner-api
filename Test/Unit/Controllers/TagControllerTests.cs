using FoodplannerApi.Controller;
using FoodplannerModels.Auth;
using FoodplannerModels.Image;
using FoodplannerServices.Image;
using FoodplannerServices.Tags;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Test.Unit.Controller;

public class TagControllerTests
{
    private readonly Mock<ITagService> mockTagService = new();
    private readonly Mock<IFoodImageService> mockFoodImageService = new();
    private readonly TagController tagController;

    public TagControllerTests()
    {
        tagController = new TagController(mockTagService.Object, mockFoodImageService.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetImagesFromTags_ReturnsOkObjectResult()
    {
        // Arrange
        byte[] imageData = {1,2,3,4,5,6,7,8};
        var images = new List<FoodImage>
        {
            new() { Id = 1, ImageName = "Tomato", ImageData = imageData, ImageFileType = "jpg", Size = 3},
            new() { Id = 2, ImageName = "Cheese", ImageData = imageData, ImageFileType = "png", Size = 4},
        };
        string tags = "frugt og grønt, mejeri";
        mockTagService.Setup(service => service.GetImagesFromTagsAsync(tags)).ReturnsAsync(images);

        // Act
        var result = await tagController.GetImagesFromTags(tags);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedImages = Assert.IsAssignableFrom<IEnumerable<FoodImage>>(okResult.Value).ToList();
        Assert.Equal(2, returnedImages.Count);
        // Image 1
        Assert.Equal(1, returnedImages[0].Id);
        Assert.Equal("Tomato", returnedImages[0].ImageName);
        Assert.Equal(imageData, returnedImages[0].ImageData);
        Assert.Equal("jpg", returnedImages[0].ImageFileType);
        Assert.Equal(3, returnedImages[0].Size);
        // Image 2
        Assert.Equal(2, returnedImages[1].Id);
        Assert.Equal("Cheese", returnedImages[1].ImageName);
        Assert.Equal(imageData, returnedImages[1].ImageData);
        Assert.Equal("png", returnedImages[1].ImageFileType);
        Assert.Equal(4, returnedImages[1].Size);
        mockTagService.Verify(service => service.GetImagesFromTagsAsync(tags), Times.Once);
    }    

}