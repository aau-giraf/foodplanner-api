using System.Diagnostics;
using AutoMapper;
using FoodplannerDataAccessSql.Image;
using FoodplannerModels.Account;
using FoodplannerModels.Image;


namespace FoodplannerServices.Image;

public class FoodImageService(IImageService imageService, IFoodImageRepository foodImageRepository, IMapper _mapper) : IFoodImageService
{
    public async Task<int> CreateFoodImage(int userid, Stream imageStream, string imageName, string imageType, long imageFileSize)
    {
        var imageId = await imageService.SaveImageAsync(userid, imageStream, imageType);

        FoodImage foodImage = new FoodImage
        {
            ImageId = imageId.ToString(),
            UserId = userid,
            ImageName = imageName,
            ImageFileType = imageType,
            Size = imageFileSize
        };
        int foodImageId = await foodImageRepository.InsertAsync(foodImage);

        return foodImageId;
    }

    public async Task<FoodImageDTO> GetFoodImage(int foodImageId)
    {
        var foodImage = await foodImageRepository.GetByIdAsync(foodImageId);
        return _mapper.Map<FoodImageDTO>(foodImage);
    }

    public async Task<string> GetFoodImageLink(int foodImageId)
    {

        var foodImage = await foodImageRepository.GetByIdAsync(foodImageId);
        if (foodImage == null)
        {
            throw new KeyNotFoundException($"FoodImage with id {foodImageId} not found");
        }
        var foodImageLink = await imageService
            .LoadImagePresignedAsync(foodImage.UserId, Guid.Parse(foodImage.ImageId), foodImage.ImageFileType);
        if (foodImageLink == null)
        {
            throw new NullReferenceException("Image link could not be retrieved.");
        }
        return foodImageLink;
    }

    public async Task<bool> DeleteImage(int foodImageId)
    {

        var foodImage = await foodImageRepository.GetByIdAsync(foodImageId);
        await foodImageRepository.DeleteAsync(foodImageId);
        if (foodImage == null)
        {
            throw new KeyNotFoundException($"FoodImage with id {foodImageId} not found");
        }
        return await imageService.DeleteImageAsync(foodImage.UserId, Guid.Parse(foodImage.ImageId), foodImage.ImageFileType);
    }
}