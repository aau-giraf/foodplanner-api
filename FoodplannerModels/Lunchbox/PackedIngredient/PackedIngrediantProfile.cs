using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerModels.Lunchbox
{
    public class PackedIngredientProfile : Profile
    {
        public PackedIngredientProfile()
        {
            CreateMap<PackedIngredient, PackedIngredientProperDTO>();
            CreateMap<PackedIngredientProperDTO, PackedIngredient>();
        }
    }
}