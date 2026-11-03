

namespace FoodplannerService.Image;
public interface IPixarbayService
{
    public Task<Pixabay> getPixarbayImage (string language, string searchWord);
}