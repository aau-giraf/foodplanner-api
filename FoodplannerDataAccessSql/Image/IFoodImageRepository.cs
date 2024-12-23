﻿using FoodplannerModels.Image;

namespace FoodplannerDataAccessSql.Image;

public interface IFoodImageRepository
{
    Task<IEnumerable<FoodImage>> GetAllImagesAsync();
    Task<FoodImage> GetImageByIdAsync(int foodImageId);
    Task<int> InsertImageAsync(string imageId, int userid, string imageName, string imageType, long imageStreamLength);
    Task DeleteImageAsync(int imageId);
}