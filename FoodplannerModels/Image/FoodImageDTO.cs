public class FoodImageDTO
{
    public int Id { get; set; }
    public required string ImageId { get; set; }
    public int UserId { get; set; }
    public required string ImageName { get; set; }
    public required string ImageFileType { get; set; }
    public long ImageSize { get; set; }
}
