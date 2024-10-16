using FoodplannerModels.Image;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace FoodplannerServices.Image;

public class ImageService(IMinioClient minioClient, ILogger<ImageService> logger) : IImageService
{
    private bool _initialized;
    private static readonly string UserImageBucket = "user-images";
    private static readonly int PresignedExpiry = 604800;
    

    public async Task<Guid> SaveImageAsync(int userId, Stream imageStream, string contentType)
    {
        var imageId = Guid.NewGuid();
        string objectName = ObjectName(userId, imageId, contentType);
        EnsureInitializedAsync().Wait();
            Console.WriteLine("We here !");
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(UserImageBucket)
            .WithObject(objectName)
            .WithObjectSize(imageStream.Length)
            .WithStreamData(imageStream)
            .WithContentType("application/octet-stream");
        await minioClient.PutObjectAsync(putObjectArgs);
        var statObjectArgs = new StatObjectArgs()
            .WithBucket(UserImageBucket)
            .WithObject(objectName);
        var objectStat = await minioClient.StatObjectAsync(statObjectArgs);
        logger.LogInformation($"{objectStat.Size} bytes saved to bucket [{UserImageBucket}].");
        imageStream.Close();
        return imageId;
    }

    public async Task LoadImageStreamAsync(int userId, Guid imageId, Stream outStream)
    {
        string objectName = ObjectName(userId, imageId, "");
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(UserImageBucket)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyTo(outStream));
        var imageObject = await minioClient.GetObjectAsync(getObjectArgs);
        logger.LogInformation($"{imageObject.Size} bytes read from bucket [{UserImageBucket}].");
    }

    public async Task<string?> LoadImagePresignedAsync(int userId, Guid imageId, string contentType)
    {
        var presignedGetArgs = new PresignedGetObjectArgs()
            .WithBucket(UserImageBucket)
            .WithObject(ObjectName(userId, imageId, contentType))
            .WithExpiry(PresignedExpiry);
        var imageUrl = await minioClient.PresignedGetObjectAsync(presignedGetArgs);
        if (imageUrl == null)
        {
            logger.LogError($"{imageUrl} was not found in bucket [{UserImageBucket}].");
        }
        return imageUrl;
    }

    public async Task<bool> DeleteImageAsync(int userId, Guid imageId, string contentType)
    {
        EnsureInitializedAsync().Wait();
        
        var removeObjectArgs = new RemoveObjectArgs()
            .WithObject(ObjectName(userId, imageId, contentType))
            .WithBucket(UserImageBucket);
        await minioClient.RemoveObjectAsync(removeObjectArgs);
        
        return true;
    }

    public async Task<bool> DeleteImagesAsync(int userId, IEnumerable<Guid> imageIds)
    {
        EnsureInitializedAsync().Wait();

        var removeObjectsArgsArgs = new RemoveObjectsArgs()
            .WithObjects(imageIds.Select(id => ObjectName(userId, id, "")).ToList())
            .WithBucket(UserImageBucket);

        var errors = await minioClient.RemoveObjectsAsync(removeObjectsArgsArgs);
        if (errors == null || !errors.Any()) return true;
        
        foreach (var deleteError in errors)
        {
            if (deleteError != null) logger.LogError(deleteError.Message);
        }
        return false;
    }

    private async Task EnsureInitializedAsync()
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(UserImageBucket);
        var exists = await minioClient.BucketExistsAsync(bucketExistsArgs);
        if (exists) return ;
        logger.LogInformation($"Bucket [{UserImageBucket}] not found. Creating this bucket");
        var makeBucketArgs = new MakeBucketArgs().WithBucket(UserImageBucket);  
        await minioClient.MakeBucketAsync(makeBucketArgs);
    }

    private string ObjectName(int userId, Guid imageId, string? extension)
    {
        var ext = (extension != null) ? $".{extension.Split("/").Last()}" : string.Empty;
        return $"{userId.ToString()}/{imageId.ToString()}" + ext;
    }
}