namespace FoodplannerModels.Image;

public class FoodImage
{
    public int Id;
    public required string ImageId { get; set; }
    public int UserId { get; set; }
    public required string ImageName { get; set; }
    public required string ImageFileType { get; set; }
    public long Size { get; set; }
}