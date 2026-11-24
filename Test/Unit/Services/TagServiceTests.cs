using FoodplannerDataAccessSql.Tags;
using FoodplannerServices.Tags;
using FoodplannerModels.Tags;
using Moq;
using FoodplannerServices.Image;

namespace Test.Unit.Service;

public class TagServiceTests
{
    // Mock repositories and service
    private readonly Mock<ITagRepository> mockTagRepository = new();
    private readonly Mock<IFoodImageService> mockFoodImageService = new();
    private readonly TagService tagService;
    public TagServiceTests()
    {
        tagService = new TagService(
            mockTagRepository.Object, mockFoodImageService.Object
        );
    }


    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetImagesFromTagsAsync_Success()
    {
        // Arrange
        List<string> tagList = new List<string> {"frugt og grønt"};
        string tagString = "frugt og grønt";
        var imageIds = new List<int>(){1,2};

        mockTagRepository.Setup(repo => repo.GetImageIdsFromTagsAsync(It.Is<List<string>>(tags => tags.SequenceEqual(tagList))))
        .ReturnsAsync(imageIds);

        mockFoodImageService
        .Setup(service => service.GetFoodImage(1))
        .ReturnsAsync(new FoodImageDTO { 
            Id = 1,
            ImageData = Array.Empty<byte>(),
            ImageName = "test1.png",
            ImageFileType = "image/png"
         });

        mockFoodImageService
        .Setup(service => service.GetFoodImage(2))
        .ReturnsAsync(new FoodImageDTO { 
            Id = 2,
            ImageData = Array.Empty<byte>(),
            ImageName = "test2.png",
            ImageFileType = "image/png"
        });

        // // Act
        var result = await tagService.GetImagesFromTagsAsync(tagString);

        // // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count());
        Assert.Equal(1, resultList[0].Id);
        Assert.Equal(2, resultList[1].Id);
    }
    
    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetImagesFromTagsAsync_RepositoryError()
    {
        // Arrange
        List<string> tagsList = new List<string> {"fruit", "sliced"};
        string tagsString = "fruit, sliced";
        mockTagRepository.Setup(repo => repo.GetImageIdsFromTagsAsync(tagsList)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => tagService.GetImagesFromTagsAsync(tagsString));
    }
}