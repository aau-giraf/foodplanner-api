using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Moq;

namespace Test.Service;

public class ClassroomServiceTests 
{
    private readonly Mock<IClassroomRepository> _mockClassroomRepository;
    private readonly ClassroomService _classService;

    public ClassroomServiceTests()
    {
        _mockClassroomRepository = new Mock<IClassroomRepository>();

        _classService = new ClassroomService(
            _mockClassroomRepository.Object
        );
    }

    [Fact]
    public async Task GetAllClassroomAsync_ReturnsAllClassrooms()
    {
        // Arrange
        var expectedClassrooms = new List<Classroom>
        {
            new Classroom { ClassId = 1, ClassName = "1.A" },
            new Classroom { ClassId = 2, ClassName = "1.B" },
        };
        _mockClassroomRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedClassrooms);
        
        // Act
        var result = await _classService.GetAllClassroomAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedClassrooms.Count, result.Count());
        Assert.All(result, classroom => Assert.Contains(expectedClassrooms, c => 
                                                    c.ClassId == classroom.ClassId && 
                                                    c.ClassName == classroom.ClassName));
    }

    [Fact]
    public async Task InsertClassroomAsync_CreatesANewClassroom()
    {
        // Arrange
        var expectedId = 1;
        var createClassroom = new CreateClassroomDTO { ClassName = "1.A" };
        _mockClassroomRepository
            .Setup(repo => repo.InsertAsync(createClassroom))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _classService.InsertClassroomAsync(createClassroom);
        
        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task UpdateClassroomAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        var createClassroom = new CreateClassroomDTO { ClassName = "1.A" };
        _mockClassroomRepository
            .Setup(repo => repo.UpdateAsync(createClassroom, expectedId))
            .ReturnsAsync(expectedId);
        
        // Act
        var result = await _classService.UpdateClassroomAsync(createClassroom, expectedId);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task CheckChildrenInClassroom_ChecksValueInRepository()
    {
        // Arrange
        var id = 1;
        _mockClassroomRepository
            .Setup(repo => repo.CheckChildrenInClassroom(id))
            .ReturnsAsync(true);
        
        // Act
        var result = await _classService.CheckChildrenInClassroom(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteClassroomAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        _mockClassroomRepository
            .Setup(repo => repo.DeleteAsync(expectedId))
            .ReturnsAsync(expectedId);
        
        // Act
        var result = await _classService.DeleteClassroomAsync(expectedId);

        // Assert
        Assert.Equal(expectedId, result);
    }
}