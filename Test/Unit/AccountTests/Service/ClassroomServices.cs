using AutoMapper;
using FoodplannerModels.Account;
using FoodplannerServices.Account;
using Moq;

namespace Test.Service;

public class ClassroomServiceTests 
{
    private readonly Mock<IClassroomRepository> _mockClassroomRepository;
    private readonly ClassroomService _classService;
    private readonly Mock<IMapper> _mockMapper;

    public ClassroomServiceTests()
    {
        _mockClassroomRepository = new Mock<IClassroomRepository>();
        _mockMapper = new Mock<IMapper>();

        _classService = new ClassroomService(
            _mockClassroomRepository.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetAllClassroomAsync_ReturnsAllClassrooms()
    {
        // Arrange
        var expectedClassrooms = new List<Classroom>
        {
            new Classroom() { ClassId = 1, ClassName = "1.A" },
            new Classroom() { ClassId = 2, ClassName = "1.B" },
        };
        _mockClassroomRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedClassrooms);
        
        _mockMapper
            .Setup(m => m.Map<ClassroomDTO>(It.IsAny<Classroom>()))
            .Returns((Classroom src) => new ClassroomDTO()
            {
                ClassId = src.ClassId,
                ClassName = src.ClassName
            });
        
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
    public async Task UpdateClassroomAsync_UpdatesValueInRepository()
    {
        // Arrange
        var id = 1;
        _mockClassroomRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Classroom>()))
            .ReturnsAsync(id);
        
        _mockMapper
            .Setup(m => m.Map<Classroom>(It.IsAny<CreateClassroomDTO>()))
            .Returns((CreateClassroomDTO src) => new Classroom
            {
                ClassId = id,
                ClassName = src.ClassName
            });
        
        var createClassroomDto = new CreateClassroomDTO() { ClassName = "1.A" };
        
        // Act
        var result = await _classService.UpdateClassroomAsync(createClassroomDto, id);

        // Assert
        var expectedId = 1;
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