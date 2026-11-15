using FoodplannerModels.Image;
using FoodplannerServices.Image;
using FoodplannerServices.ExternalApi;
using Moq;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Identity;
using Test.Builder;

namespace Test.Service;

public class PixabayServiceTests
{
    private readonly Mock<IPixabayApi> _mockPixabayApi;
    private readonly Mock<IFoodImageService> _mockFoodImageService;
    private readonly Mock<IWebClientWrapper> _mockImageURL;
    private readonly PixabayService _pixabayService;

    public PixabayServiceTests()
    {
        _mockPixabayApi = new Mock<IPixabayApi>();
        _mockFoodImageService = new Mock<IFoodImageService>();
        _mockImageURL = new Mock<IWebClientWrapper>();

        _pixabayService = new PixabayService(
            _mockPixabayApi.Object,
            _mockFoodImageService.Object,
            _mockImageURL.Object
        );
    }

    // Test for: Get images from external service
    [Fact]
    public async Task GetPixabayImages_ByDefaultLanguage()
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

        _mockPixabayApi
            .Setup(api => api.SearchImages(
                "da", "æble", 20, "all", "popular", "food"))
            .ReturnsAsync(expectedImages);

        // Act
        var result = await _pixabayService.GetPixabayImage(search);

        // Assert
        Assert.Equal(expectedImages, result);
        Assert.Equal("da", search.Language);

        _mockPixabayApi.Verify(
            api => api.SearchImages(
                "da", "æble", 20, "all", "popular", "food"),
            Times.Once
        );
    }

    [Fact]
    public async Task GetPixabayImages_ByCustomLanguage()
    {
        // Arrange
        var search = new GetPixabayImageDTO
        {
            SearchWord = "apple",
            Language = "en"
        };

        var expectedImages = new Pixabay
        {
            Response = Enumerable.Range(1, 20)
                .Select(i => $"http://www.example/apple/{i}")
                .ToList()
        };

        _mockPixabayApi
            .Setup(api => api.SearchImages(
                "en", "apple", 20, "all", "popular", "food"))
            .ReturnsAsync(expectedImages);

        // Act
        var result = await _pixabayService.GetPixabayImage(search);

        // Assert
        Assert.Equal(expectedImages, result);
        Assert.Equal("en", search.Language);

        _mockPixabayApi.Verify(
            api => api.SearchImages(
                "en", "apple", 20, "all", "popular", "food"),
            Times.Once
        );
    }

    // Test for: Save URL images
    [Fact]
    public async Task SavePixabayImages_Pass()
    {
        // Arrange
        const int userId = 1;
        const string keyword = "apple";
        const string url = "https://example.com/apple.jpg";
        const int expectedImageId = 5;

        byte[] imageBytes = { 1, 2, 3, 4, 5 };

        _mockImageURL
            .Setup(d => d.DownloadData(url))
            .Returns(imageBytes);

        _mockFoodImageService
            .Setup(s => s.CreateFoodImage(
                userId,
                It.IsAny<MemoryStream>(),
                keyword,
                "image/jpeg",
                imageBytes.Length))
            .ReturnsAsync(expectedImageId);

        // Act
        var result = await _pixabayService.SavePixabayUrl(
            userId, keyword, url);

        // Assert
        Assert.Equal(expectedImageId, result);

        _mockImageURL.Verify(
            d => d.DownloadData(url),
            Times.Once
        );

        _mockFoodImageService.Verify(
            s => s.CreateFoodImage(
                userId,
                It.IsAny<MemoryStream>(),
                keyword,
                "image/jpeg",
                imageBytes.Length),
            Times.Once
        );
    }

    [Fact]
    public async Task SavePixabayImages_FailNotAURL()
    {
        // Arrange
        const int userId = 1;
        const string keyword = "apple";
        const string url = "not-a-url";

        _mockImageURL
            .Setup(d => d.DownloadData(url))
            .Throws(new UriFormatException("Invalid URL"));

        // Act & Assert
        await Assert.ThrowsAsync<UriFormatException>(
            () => _pixabayService.SavePixabayUrl(
                userId, keyword, url)
        );

        _mockFoodImageService.Verify(
            s => s.CreateFoodImage(
                It.IsAny<int>(),
                It.IsAny<MemoryStream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>()),
            Times.Never
        );
    }

    [Fact]
    public async Task SavePixabayImages_FailTooLargeImage()
    {
        // Arrange
        const int userId = 1;
        const string keyword = "apple";
        const string url = "https://example.com/apple.jpg";

        // Maximum size in this test is 100 MB
        byte[] imageBytes = new byte[100000000];

        _mockImageURL
            .Setup(d => d.DownloadData(url))
            .Returns(imageBytes);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _pixabayService.SavePixabayUrl(
                userId, keyword, url)
        );

        Assert.Equal("File is too big", exception.Message);

        _mockFoodImageService.Verify(
            s => s.CreateFoodImage(
                It.IsAny<int>(),
                It.IsAny<MemoryStream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>()),
            Times.Never
        );
    }

    [Fact]
    public async Task SavePixabayImages_FailTooSmallImage()
    {
        // Arrange
        const int userId = 1;
        const string keyword = "apple";
        const string url = "https://example.com/apple.jpg";

        byte[] imageBytes = Array.Empty<byte>();

        _mockImageURL
            .Setup(d => d.DownloadData(url))
            .Returns(imageBytes);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _pixabayService.SavePixabayUrl(
                userId, keyword, url)
        );

        Assert.Equal("File is empty", exception.Message);

        _mockFoodImageService.Verify(
            s => s.CreateFoodImage(
                It.IsAny<int>(),
                It.IsAny<MemoryStream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<long>()),
            Times.Never
        );
    }
}