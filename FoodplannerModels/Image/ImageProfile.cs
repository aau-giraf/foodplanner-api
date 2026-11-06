using Mapster;
using MapsterMapper;

namespace FoodplannerModels.Image;

public class ImageProfile : IRegister
{
        public void Register(TypeAdapterConfig config)
        {
                config.NewConfig<FoodImage, FoodImageDTO>().Map(dest => dest.ImageSize, src => src.Size);
                config.NewConfig<FoodImageDTO, FoodImage>();
        }
}