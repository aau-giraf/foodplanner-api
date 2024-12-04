using AutoMapper;
using FoodplannerApi.Helpers;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;

namespace FoodplannerServices.Account;

public class ChildrenService : IChildrenService
{
    private readonly IChildrenRepository _childrenRepository;

    private readonly IMapper _mapper;

    private readonly IAuthService _authService;


    public ChildrenService(IChildrenRepository childrenRepository, IMapper mapper, IAuthService authService)
    {
        _childrenRepository = childrenRepository;
        _mapper = mapper;
        _authService = authService;
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

    public async Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync()
    {
        var children = await _childrenRepository.GetAllChildrenClassesAsync();
        return children;
    }

    public async Task<Children> GetChildrenByIdAsync(int id)
    {
        var children = await _childrenRepository.GetByParentIdAsync(id);
        return children;
    }

    public async Task<int> UpdateChildrenAsync(Children children)
    {
        return await _childrenRepository.UpdateAsync(children);
    }

    public async Task<int> DeleteChildrenAsync(int id)
    {
        return await _childrenRepository.DeleteAsync(id);
    }
}



