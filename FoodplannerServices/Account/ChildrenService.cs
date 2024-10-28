using AutoMapper;
using FoodplannerApi.Helpers;
using FoodplannerModels.Account;

namespace FoodplannerServices.Account;

public class ChildrenService : IChildrenService {
    private readonly IChildrenRepository _childrenRepository;

    private readonly IMapper _mapper;


    public ChildrenService(IChildrenRepository childrenRepository, IMapper mapper) {
       _childrenRepository = childrenRepository;
         _mapper = mapper;
    }

    public async Task<int> CreateChildrenAsync(ChildrenCreateParentDTO childrenCreateDTO)
    {
        var children = _mapper.Map<Children>(childrenCreateDTO);
        return await _childrenRepository.InsertAsync(children);
    
    }

    public async Task<IEnumerable<Children>> GetAllChildrenAsync()
    {
        var children = await _childrenRepository.GetAllAsync();
        return children;
    }
}



