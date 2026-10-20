using AutoMapper;

namespace FoodplannerModels.Image;

public class ImageProfile : Profile
{
        public ImageProfile()
        {
                CreateMap<FoodImage, FoodImageDTO>().ForMember(dest => dest.ImageSize,opt => opt.MapFrom(src => src.Size));
                CreateMap<FoodImageDTO, FoodImage>();
        }
}