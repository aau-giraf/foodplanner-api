using FoodplannerDataAccessSql.Image;
using FoodplannerModels.Image;
using FoodplannerModels.Interfaces.ExternalApi;

namespace FoodplannerServices.Image;

public class FoodImageService(IFoodImageRepository foodImageRepository, IAiApi aiApi, ICoreApi coreApi) : IFoodImageService
{
    // Creates a new food image.
    public async Task<int> CreateFoodImage(int userid, Stream imageStream, string imageName, string imageType, long imageFileSize)
    {
        // Convert the image data to byte
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream);
        byte[] imageBytes = memoryStream.ToArray();

        FoodImage foodImage = new FoodImage
        {
            ImageData = imageBytes,
            UserId = userid,
            ImageName = imageName,
            ImageFileType = imageType,
            Size = imageFileSize
        };
        
        // Save image in DB and return its ID
        int foodImageId = await foodImageRepository.InsertAsync(foodImage);
        return foodImageId;
    }


    // Return food image by ID.
    public async Task<FoodImage> GetFoodImage(int foodImageId)
    {
        var foodImage = await foodImageRepository.GetByIdAsync(foodImageId);

        if (foodImage == null)
        {
            throw new KeyNotFoundException($"FoodImage with id {foodImageId} not found");
        }

        return foodImage;
    }

    // Get a URL for the food image by ID.
    public async Task<string> GetFoodImageLink(int foodImageId)
    {
        // Find Image in repo
        var foodImage = await foodImageRepository.GetByIdAsync(foodImageId);
        if (foodImage == null)
        {
            throw new KeyNotFoundException($"FoodImage with id {foodImageId} not found");
        }

        // Converts image data to a Base64 Data URL.
        return $"data:{foodImage.ImageFileType};base64," + Convert.ToBase64String(foodImage.ImageData);
    }

    // Delete food image by ID.
    public async void DeleteImage(int foodImageId)
    {
        await foodImageRepository.DeleteAsync(foodImageId);
    }

    // Get all food stock images
    public async Task<IEnumerable<FoodImage>> GetAllStockImagesAsync()
    {
        var images = await foodImageRepository.GetAllStockImagesAsync();
        
        return images.Select(foodImage => new FoodImage
        {
        Id = foodImage.Id,
            ImageData = foodImage.ImageData,
            UserId = foodImage.UserId,
            ImageName = foodImage.ImageName,
            ImageFileType = foodImage.ImageFileType,
            Size = foodImage.Size
        }).ToList();
    }

	// Create an AI-generated image based on a prompt.
	public async Task<string> CreateAiImage(int userId, string prompt)
	{
		// Call AI API to create an image
		var imageId = await aiApi.CreateImage(userId, prompt);

		// Wait 4 sec before calling CORE
		await Task.Delay(4000);

		// Poll CORE API for the image MAX 5 times with 1 sec delay between each call
		List<string> coreRes = null;
		for (int i = 0; i < 5; i++)
		{
			await Task.Delay(1000);

			// Get image form CORE API
			coreRes = await coreApi.GetImages(new List<string> { imageId });
			
			if (coreRes == null)
			{
				Console.WriteLine($"Image not ready yet. Attempt {i + 1}/5");
				continue;
			}

			break;
		}

		// If the image is still not available after 5 attempts, throw an exception
		if (coreRes == null || coreRes.Count != 1)
		{
			throw new InvalidOperationException("Failed to retrieve image after multiple attempts.");
		}

		return coreRes[0];
	}

}