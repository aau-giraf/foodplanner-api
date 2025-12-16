using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerModels.Lunchbox
{
    public class PackedIngredientProfile : Profile
    {
        public PackedIngredientProfile()
        {
            CreateMap<PackedIngredient, PackedIngredientDTO>();
            CreateMap<PackedIngredientDTO, PackedIngredient>();
            
            CreateMap<PackedIngredient, PackedIngredientProperDTO>();
            CreateMap<PackedIngredientProperDTO, PackedIngredient>();
        }
    }
}