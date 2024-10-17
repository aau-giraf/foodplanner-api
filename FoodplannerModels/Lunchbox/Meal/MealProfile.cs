using AutoMapper;

namespace FoodplannerModels.Lunchbox
{
    public class MealProfile : Profile
    {
        public MealProfile()
        {
            CreateMap<Meal, MealDTO>();
        }
    }
}
