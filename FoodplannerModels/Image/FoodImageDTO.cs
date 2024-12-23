﻿namespace FoodplannerModels.Image;

public class FoodImageDTO
{
    public int Id;
    public string ImageId { get; set; }
    public int UserId { get; set; }
    public string ImageName { get; set; }
    public string ImageFileType { get; set; }
    public long ImageSize { get; set; }
}