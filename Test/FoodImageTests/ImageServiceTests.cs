using Minio;
using FoodplannerServices.Image;
using Microsoft.Extensions.Logging;
using Minio.DataModel.Args;
using Moq;
namespace testing;

<<<<<<< HEAD:Test/FoodImageTests/ImageServiceTests.cs
public class ImageServiceTests
{

}
=======
public class ImageServiceTests {
    [Fact]
    public async Task DoesImageBelongToUser()
    {
        var mockMinioClient = new Mock<IMinioClient>();
        var mockLogger = new Mock<ILogger<ImageService>>();
        int userId;
        var imageId = Guid.NewGuid();
        var outStream = new MemoryStream();
        var imageData = new MemoryStream(new byte[] { 1, 2, 3 }); //simulate image data
        
        mockMinioClient
            .Setup(client => client.GetObjectAsync(It.IsAny<GetObjectArgs>()))
            .Returns(Task.CompletedTask)
            .Callback<GetObjectArgs>(args =>
            {
                args.WithCallbackStream(stream => imageData.CopyTo(outStream)); //Simulate copying image into outStream
            }); 
    
    var imageService = new ImageService(mockMinioClient.Object, mockLogger.Object);
    await imageService.LoadImageStreamAsync(userId, imageId, outStream);
    }
}
>>>>>>> 647789608d74230c16ef2b5f1f8a18cb2cc8b837:Test/ImageServiceTests.cs
