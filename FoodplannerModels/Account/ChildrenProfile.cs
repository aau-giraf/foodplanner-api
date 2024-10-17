using AutoMapper;


namespace FoodplannerModels.Account
{
    public class ChildrenProfile : Profile
    {
        public ChildrenProfile()
        {
            CreateMap<Children, ChildrenCreateDTO>();
            CreateMap<ChildrenCreateDTO, Children>();
        }
    }
}
