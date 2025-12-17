using AutoMapper;

namespace FoodplannerModels.Image;

public class ImageProfile : Profile
{
        public ImageProfile()
        {
                CreateMap<FoodImage, FoodImageDTO>();
                CreateMap<FoodImageDTO, FoodImage>();
        }
}