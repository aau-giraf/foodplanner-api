using AutoMapper;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;

namespace FoodplannerServices.Account;

public class ChildrenService : IChildrenService
{
    private readonly IChildrenRepository _childrenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;


    public ChildrenService(IChildrenRepository childrenRepository, IUserRepository userRepository, IMapper mapper, IAuthService authService)
    {
        _childrenRepository = childrenRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _authService = authService;
    }

    public async Task<int> CreateChildrenAsync(ChildrenCreateParentDTO childrenCreateDTO)
    {
        var children = _mapper.Map<Children>(childrenCreateDTO);
        var childId = await _childrenRepository.InsertAsync(children);

        foreach (var parentId in childrenCreateDTO.ParentIds)
        {
            await AddParentToChildAsync(parentId, childId);
        }

        return childId;
    }

    public async Task<IEnumerable<ChildrenDTO>> GetAllChildrenAsync()
    {
        var children = await _childrenRepository.GetAllAsync();
        var childrenDto = children.Select(child => _mapper.Map<ChildrenDTO>(child));
        return childrenDto;
    }

    public async Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync()
    {
        var children = await _childrenRepository.GetAllChildrenClassesAsync();
        return children;
    }

    public async Task<IEnumerable<ChildrenDTO>> GetChildrenByParentIdAsync(int parentId)
    {
        var children = await _childrenRepository.GetChildrenByParentIdAsync(parentId);
        return _mapper.Map<IEnumerable<ChildrenDTO>>(children);
    }

    public async Task<IEnumerable<UserDTO>> GetParentsByChildIdAsync(int childId)
    {
        var parents = await _childrenRepository.GetParentsByChildIdAsync(childId);
        return _mapper.Map<IEnumerable<UserDTO>>(parents);
    }

    public async Task<int> UpdateChildrenAsync(ChildrenDTO childrenDto)
    {
        var children = _mapper.Map<Children>(childrenDto);
        var result = await _childrenRepository.UpdateAsync(children);
        return result;
    }

    public async Task<int> DeleteChildrenAsync(int id)
    {
        return await _childrenRepository.DeleteAsync(id);
    }

    public async Task<ChildrenDTO> GetChildFromChildIdAsync(int id)
    {
        var children = await _childrenRepository.GetChildByIdAsync(id);
        return _mapper.Map<ChildrenDTO>(children);
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

    public async Task<int> AddTeacherToChildAsync(int userId, int childId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("Bruger ikke fundet");
        }

        if (user.Role != "Teacher")
        {
            throw new InvalidOperationException("Kun brugere med rolle 'Teacher' kan tilføjes som lærere");
        }

        if (!user.RoleApproved)
        {
            throw new InvalidOperationException("Brugerens rolle er ikke godkendt");
        }

        return await _childrenRepository.AddTeacherToChildAsync(userId, childId);
    }

    public async Task<int> RemoveTeacherFromChildAsync(int userId, int childId)
    {
        return await _childrenRepository.RemoveTeacherFromChildAsync(userId, childId);
    }

    public async Task<IEnumerable<UserDTO>> GetTeachersByChildIdAsync(int childId)
    {
        var teachers = await _childrenRepository.GetTeachersByChildIdAsync(childId);
        return _mapper.Map<IEnumerable<UserDTO>>(teachers);
    }

    public async Task<IEnumerable<ChildrenDTO>> GetChildrenByTeacherIdAsync(int teacherId)
    {
        var children = await _childrenRepository.GetChildrenByTeacherIdAsync(teacherId);
        return _mapper.Map<IEnumerable<ChildrenDTO>>(children);
    }
}



