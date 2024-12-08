using Moq;
using FoodplannerApi.Controller;
using FoodplannerApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FoodplannerServices.Image;
using FoodplannerModels.Image;
using FoodplannerModels.Auth;



namespace Test.Controller;

public class ImagesControllerTests
{
    [Fact]
    public async Task UploadImage_ValidTokenAndValidImage_ReturnsOkWithImageId()
    {
        //arrange
        var token = "valid-jwt-token";
        var imageFile = new Mock<IFormFile>();
        var userId = 1;
        var imageId = 12345;

        var mockFoodImageService = new Mock<IFoodImageService>();
        var mockAuthService = new Mock<IAuthService>();

        var controller = new ImagesController(mockFoodImageService.Object, mockAuthService.Object);

        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());

        mockFoodImageService.Setup(service => service.CreateFoodImage(
            userId,
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<long>()
        )).ReturnsAsync(imageId);

        //act
        var result = await controller.UploadImage(token, imageFile.Object);

        //assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(imageId, okResult.Value);
    }
    [Fact]
    public async Task UploadImages_ValidTokenAndValidImages_ReturnsOkWithImageIds()
    {
        //arrange
        var token = "valid-jwt-token";

        var mockFile1 = new Mock<IFormFile>();
        var mockFile2 = new Mock<IFormFile>();

        mockFile1.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));
        mockFile1.Setup(f => f.FileName).Returns("image1.jpg");
        mockFile1.Setup(f => f.ContentType).Returns("image/jpeg");
        mockFile1.Setup(f => f.Length).Returns(1024);

        mockFile2.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 4, 5, 6 }));
        mockFile2.Setup(f => f.FileName).Returns("image2.jpg");
        mockFile2.Setup(f => f.ContentType).Returns("image/jpeg");
        mockFile2.Setup(f => f.Length).Returns(1024);

        var mockImageFiles = new Mock<IFormFileCollection>();
        mockImageFiles.Setup(f => f.GetEnumerator()).Returns(new List<IFormFile> { mockFile1.Object, mockFile2.Object }.GetEnumerator());

        var imageFiles = mockImageFiles.Object;

        var mockFoodImageService = new Mock<IFoodImageService>();
        var userId = 1;

        var imageId1 = 12345;
        var imageId2 = 67890;
        mockFoodImageService.Setup(service => service.CreateFoodImage(
            userId,
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<long>()
        ))
        .ReturnsAsync((int userId, Stream fileStream, string fileName, string contentType, long length) =>
        {
            return fileName == "image1.jpg" ? imageId1 : imageId2;
        });

        var mockAuthService = new Mock<IAuthService>();
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());

        var controller = new ImagesController(mockFoodImageService.Object, mockAuthService.Object);

        // Act
        var result = await controller.UploadImages(imageFiles, userId);
        var okResult = Assert.IsType<OkObjectResult>(result);

        // Assert
        Assert.Equal(200, okResult.StatusCode);

        var imageIds = Assert.IsType<IEnumerable<string>>(okResult.Value);
        Assert.Contains(imageIds, id => id == imageId1.ToString());
        Assert.Contains(imageIds, id => id == imageId2.ToString());
    }

    [Fact]
    public async Task DeleteImages_ValidTokenAndValidImageIds_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var token = "valid-jwt-token";
        var foodImageIds = new List<int> { 12345, 67890 };

        var mockAuthService = new Mock<IAuthService>();
        var userId = 1;
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());

        var mockFoodImageService = new Mock<IFoodImageService>();

        mockFoodImageService.Setup(service => service.DeleteImage(It.IsAny<int>()))
            .Verifiable();

        var controller = new ImagesController(mockFoodImageService.Object, mockAuthService.Object);

        // Act
        var result = await controller.DeleteImages(foodImageIds);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("Images deleted successfully", okResult.Value);

        mockFoodImageService.Verify(service => service.DeleteImage(It.Is<int>(id => foodImageIds.Contains(id))), Times.Exactly(foodImageIds.Count));
    }

    [Fact]
    public async Task GetFoodImage_ValidTokenAndValidImageId_ReturnsOkWithImageData()
    {
        // Arrange
        var token = "valid-jwt-token";
        var foodImageId = 12345;

        var mockAuthService = new Mock<IAuthService>();
        var userId = 1;
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token)).Returns(userId.ToString());

        var mockFoodImageService = new Mock<IFoodImageService>();

        var expectedImage = new FoodImage
        {
            Id = foodImageId,
            UserId = userId,
        };

        mockFoodImageService.Setup(service => service.GetFoodImage(foodImageId)).ReturnsAsync(expectedImage);

        var controller = new ImagesController(mockFoodImageService.Object, mockAuthService.Object);

        // Act
        var result = await controller.GetFoodImage(foodImageId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var returnedImage = Assert.IsType<FoodImage>(okResult.Value);
        Assert.Equal(expectedImage.Id, returnedImage.Id);
    }

    [Fact]
    public async Task GetPresignedImageLink_ValidTokenAndValidImageId_ReturnsOkWithPresignedLink()
    {
        // Arrange
        var token = "valid-jwt-token";
        var foodImageId = 12345;

        var mockAuthService = new Mock<IAuthService>();
        var userId = 1;
        mockAuthService.Setup(auth => auth.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());

        var mockFoodImageService = new Mock<IFoodImageService>();

        var presignedLink = "http://localhos/sample-link";

        mockFoodImageService.Setup(service => service.GetFoodImageLink(foodImageId))
            .ReturnsAsync(presignedLink);

        var controller = new ImagesController(mockFoodImageService.Object, mockAuthService.Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

        // Act
        var result = await controller.GetPresignedImageLink(foodImageId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var returnedLink = Assert.IsType<string>(okResult.Value);
        Assert.Equal(presignedLink, returnedLink);
    }
}










