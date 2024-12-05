using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerModels.Lunchbox
{
    public class MealProfile : Profile
    {
        public MealProfile()
        {
            CreateMap<Meal, MealCreateDTO>();
            CreateMap<MealCreateDTO, Meal>();
        }
    }
}
