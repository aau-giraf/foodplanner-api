
using FoodplannerModels.Images;

namespace FoodplannerDataAccessSql.Images;

public class ImageRepository : IImageRepository {

    public FoodplannerImage (FoodplannerImage Image)
    {
        try
        {
            return Image;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    /*
    public GetImages(int ImageID)
    {
        try {
            var Image = new FoodplannerImage();
            // tilføje billede til image fra database
        }
        
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
    */
    
    
    
    
}