 using AutoMapper;
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
        var foodImageService = new FoodImageService(foodImageServiceFixture.FoodImageRepository, new Mock<IMapper>().Object);

         var imageStream = new MemoryStream([0]);
         var actualUserId = await foodImageService.CreateFoodImage(expectedUserId, imageStream, "someImage", "someType", imageStream.Length);
         Assert.Equal(expectedUserId, actualUserId);
     }
 }

public class FoodImageServiceFixture : IDisposable
{
    public IFoodImageRepository FoodImageRepository { get; }

    public FoodImageServiceFixture()
    {
        var dummyStream = new MemoryStream();
        var mockedFoodImageRepository = new Mock<IFoodImageRepository>();

        mockedFoodImageRepository.Setup(repo => repo.InsertAsync(It.IsAny<FoodImage>()))
            .Returns((FoodImage foodImage) => Task.FromResult(foodImage.UserId));

        FoodImageRepository = mockedFoodImageRepository.Object;
    }
    public void Dispose()
    {

     }
 }
