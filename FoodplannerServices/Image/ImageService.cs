using FoodplannerModels.Images;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace FoodplannerServices.Image;

public class ImageService(IMinioClient minioClient, ILogger<ImageService> logger) : IImageService
{
    private bool _initialized;
    private const string UserImageBucket = "user-images";

    public async Task<Guid> SaveImage(int userId, Stream imageStream)
    {
        var imageId = Guid.NewGuid();
        string objectName = ObjectName(userId, imageId);
        await EnsureInitializedAsync();
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(UserImageBucket)
            .WithObject(objectName)
            .WithObjectSize(imageStream.Length)
            .WithStreamData(imageStream)
            .WithContentType("application/octet-stream");
        await minioClient.PutObjectAsync(putObjectArgs);
        var statObjectArgs = new StatObjectArgs().WithBucket(UserImageBucket).WithObject(objectName);
        var objectStat = await minioClient.StatObjectAsync(statObjectArgs);
        logger.LogInformation($"{objectStat.Size} bytes saved to bucket [{UserImageBucket}].");

        return imageId;
    }

    public async Task<bool> DeleteImage(int userId, Guid imageId)
    {
        if (!await EnsureInitializedAsync()) return false;
        
        var removeObjectArgs = new RemoveObjectArgs()
            .WithObject(ObjectName(userId, imageId))
            .WithBucket(UserImageBucket);
        await minioClient.RemoveObjectAsync(removeObjectArgs);
        
        return true;
    }

    public async Task<bool> DeleteImages(int userId, IEnumerable<Guid> imageIds)
    {
        if (!await EnsureInitializedAsync()) return false;

        var removeObjectsArgsArgs = new RemoveObjectsArgs()
            .WithObjects(imageIds.Select(id => id.ToString()).ToList())
            .WithBucket(UserImageBucket);

        var errors = await minioClient.RemoveObjectsAsync(removeObjectsArgsArgs);
        if (errors == null || !errors.Any()) return true;
        
        foreach (var deleteError in errors)
        {
            if (deleteError != null) logger.LogError(deleteError.Message);
        }
        return false;
    }
    

    private async Task<bool> EnsureInitializedAsync()
    {
        if (_initialized) return true;
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(UserImageBucket);
        var exists = await minioClient.BucketExistsAsync(bucketExistsArgs);
        if (exists) return _initialized = true;
        logger.LogInformation($"Bucket [{UserImageBucket}] not found. Creating this bucket");
        var makeBucketArgs = new MakeBucketArgs().WithBucket(UserImageBucket);
        await minioClient.MakeBucketAsync(makeBucketArgs);
        return _initialized = true;
    }

    private string ObjectName(int userId, Guid imageId)
    {
        return $"{userId.ToString()}/{imageId.ToString()}";
    }
}