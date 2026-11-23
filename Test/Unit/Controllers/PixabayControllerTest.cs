using Moq;
using FoodplannerApi.Controller;
using FoodplannerServices.Image;
using FoodplannerModels.Image;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

namespace Test.AccountTests.Controller;

public class PixabayControllerTests
{
    private readonly Mock<IPixabayService> _pixabayServiceMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly PixabayController _controller;

    public PixabayControllerTests()
    {
        _pixabayServiceMock = new Mock<IPixabayService>();
        _authServiceMock = new Mock<IAuthService>();

        _controller = new PixabayController(
            _pixabayServiceMock.Object,
            _authServiceMock.Object
        );
    }

    // Get images
    [Fact]
    public async Task GetImages_ReturnsOkObjectResult_WhenSearchWordIsValid()
    {
        // Arrange
        var search = new GetPixabayImageDTO
        {
            SearchWord = "æble"
        };

        var expectedImages = new Pixabay
        {
            Response = Enumerable.Range(1, 20)
                .Select(i => $"http://www.example/æble/{i}")
                .ToList()
        };

        _pixabayServiceMock
            .Setup(s => s.GetPixabayImage(search))
            .ReturnsAsync(expectedImages);

        // Act
        var result = await _controller.Get(search);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(expectedImages, okResult.Value);

        _pixabayServiceMock.Verify(
            s => s.GetPixabayImage(search),
            Times.Once
        );
    }

    [Fact]
    public async Task GetImages_ReturnsNotFound_WhenSearchReturnsEmptyList()
    {
        // Arrange
        var search = new GetPixabayImageDTO
        {
            SearchWord = "æble",
            Language = "en"
        };

        _pixabayServiceMock
            .Setup(s => s.GetPixabayImage(search))
            .ReturnsAsync(new Pixabay
                {
                    Response = new List<string>()
                });

        // Act
        var result = await _controller.Get(search);

        // Assert
        Assert.IsType<OkObjectResult>(result);

        _pixabayServiceMock.Verify(
            s => s.GetPixabayImage(search),
            Times.Once
        );
    }

    // Save images
    [Fact]
    public async Task SaveImages_ReturnsOkObjectResult_WhenUrlIsValid()
    {
        // Arrange
        const string token = "valid-token";
        const string idString = "1";
        const int userId = 1;
        const int expectedImageId = 10;
    

        var pixabayDTO = new SavePixabayImageDTO
        {
            SearchWord = "apple",
            Url = "https://example.com/apple.jpg",
            ingredientId = new IngredientDTO
            {
                Id = 1, Name = "apple", User_id = userId
            },
            categoryIds = new CategoryDTO 
            { 
                Id = 1, 
                Name = "frugt", 
                Main = true
            }
        };

        _authServiceMock
            .Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(idString);

        _pixabayServiceMock
            .Setup(s => s.SavePixabayUrl(
                userId,
                pixabayDTO.SearchWord,
                pixabayDTO.Url))
            .ReturnsAsync(expectedImageId);

        // Act
        var result = await _controller.SavePixabayUrl(token, pixabayDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(expectedImageId, okResult.Value);

        _authServiceMock.Verify(
            a => a.RetrieveIdFromJwtToken(token),
            Times.Once
        );

        _pixabayServiceMock.Verify(
            s => s.SavePixabayUrl(
                userId,
                pixabayDTO.SearchWord,
                pixabayDTO.Url),
            Times.Once
        );
    }

    [Fact]
    public async Task SaveImages_ReturnsBadRequest_WhenUserIdIsInvalid()
    {
        // Arrange
        const string token = "invalid-token";

        var pixabayDTO = new SavePixabayImageDTO
        {
            SearchWord = "apple",
            Url = "https://example.com/apple.jpg", 
                       ingredientId = new IngredientDTO
            {
                Id = 1, Name = "apple", User_id = 1
            },
            categoryIds = new CategoryDTO 
            { 
                Id = 1, 
                Name = "frugt", 
                Main = true
            }
        };

        _authServiceMock
            .Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns("invalid-id");

        // Act
        var result = await _controller.SavePixabayUrl(token, pixabayDTO);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        dynamic value = badRequest.Value!;
        Assert.Contains("Invalid user ID", value.Message);

        _pixabayServiceMock.Verify(
            s => s.SavePixabayUrl(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task SaveImages_ReturnsBadRequest_WhenServiceThrowsInvalidOperationException()
    {
        // Arrange
        const string token = "valid-token";
        const int userId = 1;
        const string errorMessage = "Unable to save image";

        var pixabayDTO = new SavePixabayImageDTO
        {
            SearchWord = "apple",
            Url = "https://example.com/apple.jpg",
                        ingredientId = new IngredientDTO
            {
                Id = 1, Name = "apple", User_id = userId
            },
            categoryIds = new CategoryDTO 
            { 
                Id = 1, 
                Name = "frugt", 
                Main = true
            }
        };

        _authServiceMock
            .Setup(a => a.RetrieveIdFromJwtToken(token))
            .Returns(userId.ToString());

        _pixabayServiceMock
            .Setup(s => s.SavePixabayUrl(
                userId,
                pixabayDTO.SearchWord,
                pixabayDTO.Url))
            .ThrowsAsync(new InvalidOperationException(errorMessage));

        // Act
        var result = await _controller.SavePixabayUrl(token, pixabayDTO);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        dynamic value = badRequest.Value!;
        Assert.Contains(errorMessage, value.Message);
    }
}
