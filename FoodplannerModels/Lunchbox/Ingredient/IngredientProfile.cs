using AutoMapper;

namespace FoodplannerModels.Lunchbox;

public class IngredientProfile: Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDTO>();
        CreateMap<IngredientDTO, Ingredient>();
    }
        
}
