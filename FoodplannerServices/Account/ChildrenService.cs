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
    private readonly IAuthService _authService;


    // Constructor 
    public ChildrenService(IChildrenRepository childrenRepository, IUserRepository userRepository, IAuthService authService)
    {
        _childrenRepository = childrenRepository;
        _userRepository = userRepository;
        _authService = authService;
    }

    // Creates a child based on childrenCreateDto (first and last-name, parents, and class)
    public async Task<int> CreateChildrenAsync(
        int childUserId,
        ChildrenCreateParentDTO childrenCreateDto)
    {
        // Check that the child user exists
        var user = await _userRepository.GetByIdAsync(childUserId);

        if (user == null || !user.Role.HasFlag(UserRole.Child))
        {
            throw new InvalidOperationException("Child user not found or invalid role");
        }

        // Manually map DTO to Children
        var child = new Children
        {
            ChildId = childUserId,
            FirstName = childrenCreateDto.FirstName,
            LastName = childrenCreateDto.LastName,
            ClassId = childrenCreateDto.ClassId
        };

        // Insert child into database
        var childId = await _childrenRepository.InsertAsync(child);

        // Add parents to child
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
        return children.Select(ToChildrenDTO).ToList();
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
        return children.Select(ToChildrenDTO).ToList();
    }

    // Retrieves parents associated with a child's ID
    public async Task<IEnumerable<UserDTO>> GetParentsByChildIdAsync(int childId)
    {
        var teachers = await _childrenRepository.GetParentsByChildIdAsync(childId);
        return teachers.Select(ToUserDTO).ToList();
    }

    // Updates a child's information in the database 
    public async Task<int> UpdateChildrenAsync(ChildrenDTO childrenDto)
    {
        var child = new Children
        {
            ChildId = childrenDto.ChildId,
            FirstName = childrenDto.FirstName,
            LastName = childrenDto.LastName,
            ClassId = childrenDto.ClassId
        };

        return await _childrenRepository.UpdateAsync(child);
    }

    // Deletes a child from the database
    public async Task<int> DeleteChildrenAsync(int id)
    {
        return await _childrenRepository.DeleteAsync(id);
    }

    // Retrieves child's information associated with child's ID
    public async Task<ChildrenDTO> GetChildFromChildIdAsync(int id)
    {
        var child = await _childrenRepository.GetByIdAsync(id);

        if (child == null)
        {
            throw new KeyNotFoundException($"Child with ID {id} not found");
        }

        return ToChildrenDTO(child);
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

        return await _childrenRepository.AddUserToChildAsync(userId, childId);
    }

    // Remove parents from child 
    public async Task<int> RemoveParentFromChildAsync(int userId, int childId)
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
            throw new InvalidOperationException("Brugeren har ikke rollen 'Parent'");
        }
        return await _childrenRepository.RemoveUserFromChildAsync(userId, childId);
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

        return await _childrenRepository.AddUserToChildAsync(userId, childId);
    }

    // Removes Teacher from Child 
    public async Task<int> RemoveTeacherFromChildAsync(int userId, int childId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        // Throws an exception if the user is not found
        if (user == null)
        {
            throw new InvalidOperationException("Bruger ikke fundet");
        } 
        
        // Throws an exception if the user does not have the parent role
        if (!user.Role.HasFlag(UserRole.Teacher))
        {
            throw new InvalidOperationException("Brugeren har ikke rollen 'Teacher'");
        }

        return await _childrenRepository.RemoveUserFromChildAsync(userId, childId);
    }

    // Retrieves teachers associated with child's ID
    public async Task<IEnumerable<UserDTO>> GetTeachersByChildIdAsync(int childId)
    {
        var teachers = await _childrenRepository.GetTeachersByChildIdAsync(childId);
        return teachers.Select(ToUserDTO).ToList();
    }

    // Retrives Children associated with Teacher's ID
    public async Task<IEnumerable<ChildrenDTO>> GetChildrenByTeacherIdAsync(int teacherId)
    {
        var children = await _childrenRepository.GetChildrenByTeacherIdAsync(teacherId);
        return children.Select(ToChildrenDTO).ToList();
    }


    public static ChildrenDTO ToChildrenDTO(Children child)
    {
        return new ChildrenDTO
        {
            ChildId = child.ChildId,
            FirstName = child.FirstName,
            LastName = child.LastName,
            ClassId = child.ClassId ?? 0
        };
    }

    // Manually maps User to UserDTO
    private static UserDTO ToUserDTO(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            RoleApproved = user.RoleApproved,
            Archived = user.Archived
        };
    }
}



