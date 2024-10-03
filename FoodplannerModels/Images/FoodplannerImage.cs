namespace FoodplannerModels.Images;

public class FoodplannerImage
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    //Don't know if this info in needed in an image object. 
    private int Width { get; set; }
    private int Height { get; set; }
    
}