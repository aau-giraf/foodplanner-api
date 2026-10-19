using System.Diagnostics;
using AutoMapper;
using FoodplannerDataAccessSql.Image;
using FoodplannerModels.Account;
using FoodplannerModels.Image;


namespace FoodplannerServices.Image;

// Service implementation for FoodImage business logic
public class FoodImageService(IFoodImageRepository foodImageRepository, IMapper _mapper) : IFoodImageService
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
        
        // Save image in DB and return it's ID
        int foodImageId = await foodImageRepository.InsertAsync(foodImage);
        return foodImageId;
    }

    // Return food image by ID.
    public async Task<FoodImageDTO> GetFoodImage(int foodImageId)
    {
        var foodImage = await foodImageRepository.GetByIdAsync(foodImageId);
        return _mapper.Map<FoodImageDTO>(foodImage);
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
        Console.WriteLine(foodImage.ImageFileType);

        // Converts image data to a Base64 Data URL.
        return $"data:{foodImage.ImageFileType};base64," + Convert.ToBase64String(foodImage.ImageData);
    }

    // Delete food image by ID.
    public async void DeleteImage(int foodImageId)
    {
        await foodImageRepository.DeleteAsync(foodImageId);
    }
}