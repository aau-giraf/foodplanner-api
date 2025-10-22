using AutoMapper;


namespace FoodplannerModels.Account
{
    public class ChildrenProfile : Profile
    {
        public ChildrenProfile()
        {
            CreateMap<Children, ChildrenCreateParentDTO>()
                .ForMember(dest => dest.ParentIds, opt => opt.Ignore());
            CreateMap<ChildrenCreateParentDTO, Children>()
                .ForMember(dest => dest.ChildId, opt => opt.Ignore()); // ChildId is auto-generated
        }
    }
}
