using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerModels.Lunchbox
{
    public class MealProfile : Profile
    {
        public MealProfile()
        {
            CreateMap<Meal, MealDTO>();
            CreateMap<MealDTO, Meal>();
            CreateMap<Meal, MealCreateDTO>();
            CreateMap<MealCreateDTO, Meal>();
        }
    }
}
