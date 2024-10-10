namespace FoodplannerServices.Image;

public interface IImageService
{
    /// <summary>
    /// SaveImageAsync saves an image stream to the s3 storage.
    /// </summary>
    /// <param name="userId">ID of the user, who owns the image.</param>
    /// <param name="imageStream">The imageStream to save.</param>
    /// <param name="contentType"></param>
    /// <returns>The GUID of the stored object, that contains the image.</returns>
    public Task<Guid> SaveImageAsync(int userId, Stream imageStream, string contentType);

    /// <summary>
    /// LoadImage loads an image from s3 storage to a Stream.
    /// </summary>
    /// <param name="userId">ID of the user, who owns the image.</param>
    /// <param name="imageId">ID of the image object to load.</param>
    /// <param name="outStream"></param>
    /// <returns>A Stream containing the image data.</returns>
    public Task LoadImageStreamAsync(int userId, Guid imageId, Stream outStream);

    /// <summary>
    /// DeleteImageAsync deletes an image from storage.
    /// </summary>
    /// <param name="userId">ID of the user, who owns the image.</param>
    /// <param name="imageId">ID of the image to delete.</param>
    /// <param name="contentType"></param>
    /// <returns>A boolean result, true if successful and otherwise false.</returns>
    public Task<string?> LoadImagePresignedAsync(int userId, Guid imageId, string contentType);
    public Task<bool> DeleteImageAsync(int userId, Guid imageId, string contentType);
    /// <summary>
    /// DeleteImagesAsync deletes multiple images from store.
    /// </summary>
    /// <param name="userId">ID of the user, who owns the images.</param>
    /// <param name="imageIds">A list of ID's of images to delete.</param>
    /// <returns></returns>
    public Task<bool> DeleteImagesAsync(int userId, IEnumerable<Guid> imageIds);
}