using System.ComponentModel.DataAnnotations.Schema;

namespace FoodplannerModels.Images;

public class FoodImageDTO
{
    public int Id;
    public string ImageId { get; set; }
    public int UserId { get; set; }
    public string ImageName { get; set; }
    public string ImageFileType { get; set; }
    public long ImageSize { get; set; }

    public FoodImageDTO(String imageId, int UserId , String imageName, String imageFileType, long imageSize)
    {
        this.ImageId = imageId;
        this.UserId = UserId;
        this.ImageName = imageName;
        this.ImageFileType = imageFileType;
        this.ImageSize = imageSize;
    }
   
}