using AutoMapper;


namespace FoodplannerModels.Account
{
    public class ChildrenProfile : Profile
    {
        public ChildrenProfile()
        {
            CreateMap<Children, ChildrenCreateParentDTO>();
            CreateMap<ChildrenCreateParentDTO, Children>();
        }
    }
}
