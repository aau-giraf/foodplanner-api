using System.Net.Mime;
using FoodplannerModels.Images;

namespace FoodplannerDataAccessSql.Images;

public interface IImageRepository
{
    public ImageRepository ImageRepository();
    
    
    
   //Idk this may need to go to the shadow realm - LeUnio
    public int SetImage(int ImageId) { return ImageId; }
    public int GetImage(int ImageID) { return ImageID; }
    public int DeleteImage(int ImageID) { return ImageID; }
}