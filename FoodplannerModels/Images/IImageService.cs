using System.Net.Mime;

namespace FoodplannerModels.Images;

public interface IImageService
{
    /// <summary>
    /// SaveImage saves an image stream to the s3 storage.
    /// </summary>
    /// <param name="userId">ID of the user, who owns the image.</param>
    /// <param name="imageStream">The imageStream to save.</param>
    /// <returns>The GUID of the stored object, that contains the image.</returns>
    public Task<Guid> SaveImage(int userId, Stream imageStream);
    /// <summary>
    /// DeleteImage deletes an image from storage.
    /// </summary>
    /// <param name="userId">ID of the user, who owns the image.</param>
    /// <param name="imageId">ID of the image to delete.</param>
    /// <returns>A boolean result, true if successful and otherwise false.</returns>
    public Task<bool> DeleteImage(int userId, Guid imageId);
    /// <summary>
    /// DeleteImages deletes multiple images from store.
    /// </summary>
    /// <param name="userId">ID of the user, who owns the images.</param>
    /// <param name="imageIds">A list of ID's of images to delete.</param>
    /// <returns></returns>
    public Task<bool> DeleteImages(int userId, IEnumerable<Guid> imageIds);
}