using AutoMapper;


namespace FoodplannerModels.Account
{
    public class ChildrenProfile : Profile
    {
        public ChildrenProfile()
        {
            CreateMap<Children, ChildrenDTO>();
            CreateMap<ChildrenDTO, Children>();
            
            CreateMap<Children, ChildrenCreateParentDTO>();
            CreateMap<ChildrenCreateParentDTO, Children>();

            CreateMap<Children, ChildrenGetAllDTO>();
            CreateMap<ChildrenGetAllDTO, Children>();
        }
    }
}
