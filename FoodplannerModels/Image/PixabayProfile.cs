using AutoMapper;

namespace FoodplannerModels.Image;
public class PixabayProfile : Profile
{
    public PixabayProfile()
    {
        CreateMap<Pixabay, PixabayDTO>();
        CreateMap<PixabayDTO, Pixabay>();
    }
}