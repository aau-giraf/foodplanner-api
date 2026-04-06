using AutoMapper;

namespace FoodplannerModels.Lunchbox;

public class SubIngredientProfile : Profile
{
    public SubIngredientProfile()
    {
        CreateMap<SubIngredientRelation, SubIngredientRelationProperDTO>();
        CreateMap<SubIngredientRelationProperDTO, SubIngredientRelation>();
    }
}