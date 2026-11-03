using System.ComponentModel;
using System.Runtime;
using AutoMapper;

namespace FoodplannerModels.Image;
public class PixabayProfile : ProfileOptimization
{
    public PixabayProfile()
    {
        CreateMap<Pixabay, PixabayDTO>();
        CreateMap<PixabayDTO, Pixabay>();
    }
}