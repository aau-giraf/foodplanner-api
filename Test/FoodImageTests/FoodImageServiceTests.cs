using FoodplannerDataAccessSql.Image;
using FoodplannerModels.Image;
using FoodplannerServices.Image;
using Microsoft.Extensions.Logging;
using Moq;

namespace testing;

public class FoodImageServiceTests(FoodImageServiceFixture foodImageServiceFixture) : IClassFixture<FoodImageServiceFixture>
{
    
    [Fact]
    public async void FoodImageIsCreatedWithCorrectUserId()
    {
        var expectedUserId = 42;
        var foodImageService = new FoodImageService(foodImageServiceFixture.ImageService, foodImageServiceFixture.FoodImageRepository);
            
        var imageStream = new MemoryStream([0]);
        var actualUserId = await foodImageService.CreateFoodImage(expectedUserId, imageStream, "someImage", "someType", imageStream.Length);
        Assert.Equal(expectedUserId, actualUserId);
    }
}

public class FoodImageServiceFixture : IDisposable
{
    public IFoodImageRepository FoodImageRepository { get; }
    public IImageService ImageService { get;}

    public FoodImageServiceFixture()
    {
        var dummyStream = new MemoryStream();
        
        var mockedImageService = new Mock<IImageService>();
        var mockedFoodImageRepository = new Mock<IFoodImageRepository>();

        mockedImageService.Setup(service => service.SaveImageAsync(It.IsAny<int>(), dummyStream, It.IsAny<string>()))
            .Returns(() => Task.FromResult(Guid.NewGuid()));
        
        mockedImageService.Setup(service => service.LoadImagePresignedAsync(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(() => Task.FromResult("https://localhost:0000/foodplanner/images/food.jpg"));
        
        mockedFoodImageRepository.Setup(repo => repo.InsertImageAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
            .Returns((string _, int id, string _, string _, long _) => Task.FromResult(id));
        
        ImageService = mockedImageService.Object;
        FoodImageRepository = mockedFoodImageRepository.Object;
        
        
    }
    public void Dispose()
    {
        
    }
}
