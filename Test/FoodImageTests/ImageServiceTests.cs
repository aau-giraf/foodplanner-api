using System.Net;
using Minio;
using FoodplannerServices.Image;
using Microsoft.Extensions.Logging;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Moq;
namespace testing;

public class ImageServiceTests(ImageServiceFixture imageServiceFixture) : IClassFixture<ImageServiceFixture>
{
    [Fact]
    public async Task CanUploadFromUninitializedState()
    {
        var imageService = new ImageService(imageServiceFixture.MinioClient.Object, Mock.Of<ILogger<ImageService>>());
        imageService.SaveImageAsync(0, new MemoryStream(), "type/type").Wait();
        
        imageServiceFixture.VerifyBucketCreated();
    }
}

public class ImageServiceFixture : IDisposable
{
    public Mock<IMinioClient> MinioClient { get; }

    private bool _bucketExists;
    private Dictionary<string, string> _bucket = new();

    public ImageServiceFixture()
    {
        var mockedMinioClient = new Mock<IMinioClient>();

        mockedMinioClient.Setup(client => client.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), CancellationToken.None))
            .Returns(Task.FromResult(_bucketExists)).Verifiable();

        mockedMinioClient.Setup(client => client.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), CancellationToken.None))
            .Returns(() =>
            {
                _bucketExists = true;
                return Task.CompletedTask;
            }).Verifiable();

        mockedMinioClient.Setup(client => client.PutObjectAsync(It.IsAny<PutObjectArgs>(), CancellationToken.None))
            .ReturnsAsync(new PutObjectResponse(
                HttpStatusCode.OK, 
                "some content", 
                new Dictionary<string, string>(), 
                1, 
                "some/object")).Verifiable();

        mockedMinioClient.Setup(client => client.StatObjectAsync(It.IsAny<StatObjectArgs>(), CancellationToken.None))
            .ReturnsAsync(ObjectStat.FromResponseHeaders("some/object", new Dictionary<string, string>()))
            .Verifiable();
        
        MinioClient = mockedMinioClient;
    }

    public void Dispose() => MinioClient.Object.Dispose();

    public void VerifyBucketCreated()
    {
        MinioClient.Verify(service => service.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), CancellationToken.None), Times.AtLeastOnce);
        MinioClient.Verify(service => service.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), CancellationToken.None), Times.Once);
        if (!_bucketExists)
        {
            Assert.Fail("Bucket doesn't exist");
        }
    }    
    
    private Task FailureOnNoBucket()
    {
        if (!_bucketExists)
        {
            Assert.Fail("Bucket was not created");
        }
        return Task.CompletedTask;
    }
}