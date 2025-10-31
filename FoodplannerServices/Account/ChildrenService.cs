using AutoMapper;
using FoodplannerApi.Helpers;
using FoodplannerModels.Account;

namespace FoodplannerServices.Account;

public class ChildrenService : IChildrenService
{
    private readonly IChildrenRepository _childrenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly AuthService _authService;


    public ChildrenService(IChildrenRepository childrenRepository, IUserRepository userRepository, IMapper mapper, AuthService authService)
    {
        _childrenRepository = childrenRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _authService = authService;
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

    public async Task<IEnumerable<Children>> GetChildrenByParentIdAsync(int parentId)
    {
        var children = await _childrenRepository.GetChildrenByParentIdAsync(parentId);
        return children;
    }

    public async Task<IEnumerable<User>> GetParentsByChildIdAsync(int childId)
    {
        var parents = await _childrenRepository.GetParentsByChildIdAsync(childId);
        return parents;
    }

    public async Task<int> UpdateChildrenAsync(Children children)
    {
        return await _childrenRepository.UpdateAsync(children);
    }

    public async Task<Children> GetChildFromChildIdAsync(int id)
    {
        return await _childrenRepository.GetChildByIdAsync(id);
    }

    public async Task<int> AddParentToChildAsync(int userId, int childId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("Bruger ikke fundet");
        }

        if (user.Role != "Parent")
        {
            throw new InvalidOperationException("Kun brugere med rolle 'Parent' kan tilføjes som forældre");
        }

        if (!user.RoleApproved)
        {
            throw new InvalidOperationException("Brugerens rolle er ikke godkendt");
        }

        return await _childrenRepository.AddParentToChildAsync(userId, childId);
    }

    public async Task<int> RemoveParentFromChildAsync(int userId, int childId)
    {
        return await _childrenRepository.RemoveParentFromChildAsync(userId, childId);
    }
}



