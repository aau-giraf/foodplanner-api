using System.ComponentModel.DataAnnotations.Schema;

namespace FoodplannerModels.Images;

public class FoodImageDTO
{
    public required int Id { get; set; }
    public required string ImageId { get; set; }
    public required int UserId { get; set; }
    public string ImageName { get; set; }
    public required string ImageFileType { get; set; }
    public required long ImageSize { get; set; }
    
    
}