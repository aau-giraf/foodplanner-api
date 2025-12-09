namespace testing;
using System.Data;
using Dapper;
using Moq;
using Xunit;
using FoodplannerDataAccessSql;
using FoodplannerModels.Account;

public class RepositoryTests
{
    private readonly IUserRepository _userRepository;
    private readonly IChildrenRepository _childrenRepository;
    private readonly IClassroomRepository _classroomRepository;

    public RepositoryTests()
    {
        _userRepository = new Mock<IUserRepository>().Object;
        _childrenRepository = new Mock<IChildrenRepository>().Object;
        _classroomRepository = new Mock<IClassroomRepository>().Object;
    }


    // USER REPOSITORY TESTS

    
    [Fact]
    public async Task UserRepository_InsertAsync()
    {
        var user = new User
        {
            Id=0, FirstName="A", LastName="B",
            Email="a@b.com", Password="123",
            Role="Parent", RoleApproved=true,
            PinCode="1111", Archived=false
        };

        var UserInsertResult = await _userRepository.InsertAsync(user);
        Assert.Equal(user.Id, UserInsertResult);

    }

    [Fact]
    public async Task UserRepository_UpdateAsync()
    {
        var user = new User
        {
            Id=18, FirstName="Updated", LastName="B",
            Email="u@b.com", Password="123",
            Role="Teacher", RoleApproved=true,
            PinCode="1111", Archived=false
        };

        var UserUpdateResult = await _userRepository.UpdateAsync(user);
        Assert.Equal(1, UserUpdateResult);
       
    }

    [Fact]
    public async Task UserRepository_DeleteAsync()
    {
        
        int idToDelete = 20; //take from insert maybe? makes it depend on it but still

        var UserDeleteResult = await _userRepository.DeleteAsync(idToDelete);
        Assert.Equal(1, UserDeleteResult);

    }





    // CHILDREN REPOSITORY TESTS

    
    [Fact]
    public async Task ChildrenRepository_InsertAsync()
    {
        var child = new Children
        {
            ChildId = 0,
            FirstName = "Kid",
            LastName = "One",
            classId = 3
        };

        var result = await _childrenRepository.InsertAsync(child);
        Assert.Equal(child.ChildId, result);
        


    }

    [Fact]
    public async Task ChildrenRepository_UpdateAsync()
    {
        var child = new Children
        {
            ChildId = 10,
            FirstName = "Updated",
            LastName = "Kid",
            classId = 5
        };

        
        var result = await _childrenRepository.UpdateAsync(child);
        Assert.Equal(1, result);
        



    }

    [Fact]
    public async Task ChildrenRepository_DeleteAsync()
    {
        
        var idToDelete = 9; 

        var result = await _childrenRepository.DeleteAsync(idToDelete);
        Assert.Equal(1, result);


    }





    // CLASSROOM REPOSITORY TESTS

    

    [Fact]
    public async Task ClassroomRepository_InsertAsync()
    {
        var classroom = new Classroom
        {
            ClassId = 0,
            ClassName = "A1"
        };

        
        
        var result = await _classroomRepository.InsertAsync(classroom);
        Assert.Equal(classroom.ClassId, result);



    }

    [Fact]
    public async Task ClassroomRepository_UpdateAsync()
    {
        var classroom = new Classroom
        {
            ClassId = 1,
            ClassName = "Updated"
        };

        
        var result = await _classroomRepository.UpdateAsync(classroom);
        Assert.Equal(1, result);
        



    }

    [Fact]
    public async Task ClassroomRepository_DeleteAsync()
    {
        
        var idToDelete = 1;
        
        var result = await _classroomRepository.DeleteAsync(idToDelete);
        Assert.Equal(1, result);



    }
}
