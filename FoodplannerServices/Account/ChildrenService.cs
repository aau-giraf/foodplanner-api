using FoodplannerModels.Account;

namespace FoodplannerServices.Account;

public class ChildrenService(IChildrenRepository childrenRepository, IUserRepository userRepository) : IChildrenService
{
    // Retrieves children from database
    public async Task<IEnumerable<ChildrenDTO>> GetAllChildrenAsync()
    {

        var children = await childrenRepository.GetAllAsync();
        // Maps child to ChildrenDto
        return children.Select(ToChildrenDTO).ToList();
    }

    // Retrieves children associated with a parent ID
    public async Task<IEnumerable<ChildrenDTO>> GetChildrenByParentIdAsync(int parentId)
    {
        var children = await childrenRepository.GetChildrenByParentIdAsync(parentId);
        // Maps children to ChildrenDto 
        return children.Select(ToChildrenDTO).ToList();
    }

    // Retrieves parents associated with a child's ID
    public async Task<IEnumerable<UserDTO>> GetParentsByChildIdAsync(int childId)
    {
        var teachers = await childrenRepository.GetParentsByChildIdAsync(childId);
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

        return await childrenRepository.UpdateAsync(child);
    }

    // Retrieves child's information associated with child's ID
    public async Task<ChildrenDTO> GetChildFromChildIdAsync(int id)
    {
        var child = await childrenRepository.GetByIdAsync(id);

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
        var user = await userRepository.GetByIdAsync(userId);

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

        return await childrenRepository.AddUserToChildAsync(userId, childId);
    }

    // Remove parents from child 
    public async Task<int> RemoveParentFromChildAsync(int userId, int childId)
    {
        var user = await userRepository.GetByIdAsync(userId); 
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
        return await childrenRepository.RemoveUserFromChildAsync(userId, childId);
    }

    // Adds Teacher to Child
    // User is referred to a teacher here, throws if teacher is not found or if user is not a teacher
    public async Task<int> AddTeacherToChildAsync(int userId, int childId)
    {
        var user = await userRepository.GetByIdAsync(userId);
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

        return await childrenRepository.AddUserToChildAsync(userId, childId);
    }

    // Removes Teacher from Child 
    public async Task<int> RemoveTeacherFromChildAsync(int userId, int childId)
    {
        var user = await userRepository.GetByIdAsync(userId);
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

        return await childrenRepository.RemoveUserFromChildAsync(userId, childId);
    }

    // Retrieves teachers associated with child's ID
    public async Task<IEnumerable<UserDTO>> GetTeachersByChildIdAsync(int childId)
    {
        var teachers = await childrenRepository.GetTeachersByChildIdAsync(childId);
        return teachers.Select(ToUserDTO).ToList();
    }

    // Retrives Children associated with Teacher's ID
    public async Task<IEnumerable<ChildrenDTO>> GetChildrenByTeacherIdAsync(int teacherId)
    {
        var children = await childrenRepository.GetChildrenByTeacherIdAsync(teacherId);
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



