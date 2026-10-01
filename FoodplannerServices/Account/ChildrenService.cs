using AutoMapper;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;

// Adds the ChildrenService to the FoodplannerService.Account namespace 
namespace FoodplannerServices.Account;

// ChildrenService Class implemented with Interface IChildrenService
public class ChildrenService : IChildrenService
{
    //Read only fields 
    private readonly IChildrenRepository _childrenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;


    // Constructor 
    public ChildrenService(IChildrenRepository childrenRepository, IUserRepository userRepository, IMapper mapper, IAuthService authService)
    {
        _childrenRepository = childrenRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _authService = authService;
    }

    // Creates a child based on childrenCreateDto (first and last-name, parents, and class)
    public async Task<int> CreateChildrenAsync(ChildrenCreateParentDTO childrenCreateDto)
    {
        // Maps childrenCreateDto to Children 
        var children = _mapper.Map<Children>(childrenCreateDto);

        // Inserts the child into the database and gives it an Id
        var childId = await _childrenRepository.InsertAsync(children);

        // Adds parents to child
        foreach (var parentId in childrenCreateDto.ParentIds)
        {
            await AddParentToChildAsync(parentId, childId);
        }

        return childId;
    }

    // Retrieves children from database
    public async Task<IEnumerable<ChildrenDTO>> GetAllChildrenAsync()
    {

        var children = await _childrenRepository.GetAllAsync();
        // Maps child to ChildrenDto
        var childrenDto = children.Select(child => _mapper.Map<ChildrenDTO>(child));
        return childrenDto;
    }


    // Retrieves all data for children, their classes and parents 
    public async Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync()
    {
        var children = await _childrenRepository.GetAllChildrenClassesAsync();
        return children;
    }

    // Retrieves children associated with a parent ID
    public async Task<IEnumerable<ChildrenDTO>> GetChildrenByParentIdAsync(int parentId)
    {
        var children = await _childrenRepository.GetChildrenByParentIdAsync(parentId);
        // Maps children to ChildrenDto 
        return _mapper.Map<IEnumerable<ChildrenDTO>>(children);
    }

    // Retrieves parents associated with a child's ID
    public async Task<IEnumerable<UserDTO>> GetParentsByChildIdAsync(int childId)
    {
        var parents = await _childrenRepository.GetParentsByChildIdAsync(childId);
        return _mapper.Map<IEnumerable<UserDTO>>(parents);
    }

    // Updates a child's information in the database 
    public async Task<int> UpdateChildrenAsync(ChildrenDTO childrenDto)
    {
        var children = _mapper.Map<Children>(childrenDto);
        var result = await _childrenRepository.UpdateAsync(children);
        return result;
    }

    // Deletes a child from the database
    public async Task<int> DeleteChildrenAsync(int id)
    {
        return await _childrenRepository.DeleteAsync(id);
    }

    // Retrieves child's information associated with child's ID
    public async Task<ChildrenDTO> GetChildFromChildIdAsync(int id)
    {
        var children = await _childrenRepository.GetByIdAsync(id);
        return _mapper.Map<ChildrenDTO>(children);
    }

    // Adds a parent to a child
    // User is referred to a parent here
    public async Task<int> AddParentToChildAsync(int userId, int childId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        // Throws an exception if the user is not found
        if (user == null)
        {
            throw new InvalidOperationException("Bruger ikke fundet");
        }

        // Throws an exception if the user does not have the parent role
        if (!user.Role.HasFlag(UserRole.Parent))
        {
            throw new InvalidOperationException("Kun brugere med rolle 'Parent' kan tilføjes som forældre");
        }

        // Throws an exception for user with non-approved role
        if (!user.RoleApproved)
        {
            throw new InvalidOperationException("Brugerens rolle er ikke godkendt");
        }

        return await _childrenRepository.AddParentToChildAsync(userId, childId);
    }

    // Remove parents from child 
    public async Task<int> RemoveParentFromChildAsync(int userId, int childId)
    {
        return await _childrenRepository.RemoveParentFromChildAsync(userId, childId);
    }

    // Adds Teacher to Child
    // User is referred to a teacher here, throws if teacher is not found or if user is not a teacher
    public async Task<int> AddTeacherToChildAsync(int userId, int childId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("Bruger ikke fundet");
        }

        if (!user.Role.HasFlag(UserRole.Teacher))
        {
            throw new InvalidOperationException("Kun brugere med rolle 'Teacher' kan tilføjes som lærere");
        }

        if (!user.RoleApproved)
        {
            throw new InvalidOperationException("Brugerens rolle er ikke godkendt");
        }

        return await _childrenRepository.AddTeacherToChildAsync(userId, childId);
    }

    // Removes Teacher from Child 
    public async Task<int> RemoveTeacherFromChildAsync(int userId, int childId)
    {
        return await _childrenRepository.RemoveTeacherFromChildAsync(userId, childId);
    }

    // Retrieves teachers associated with child's ID
    public async Task<IEnumerable<UserDTO>> GetTeachersByChildIdAsync(int childId)
    {
        var teachers = await _childrenRepository.GetTeachersByChildIdAsync(childId);
        return _mapper.Map<IEnumerable<UserDTO>>(teachers);
    }

    // Retrives Children associated with Teacher's ID
    public async Task<IEnumerable<ChildrenDTO>> GetChildrenByTeacherIdAsync(int teacherId)
    {
        var children = await _childrenRepository.GetChildrenByTeacherIdAsync(teacherId);
        return _mapper.Map<IEnumerable<ChildrenDTO>>(children);
    }
}



