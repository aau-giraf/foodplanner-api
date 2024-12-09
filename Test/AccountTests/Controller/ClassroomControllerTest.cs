using FoodplannerApi.Controller;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Controller;

public class ClassroomControllerTest
{
    private readonly Mock<IClassroomService> _mockClassroomService;
    private readonly ClassroomsController  _classroomController;

    public ClassroomControllerTest()
    {
        _mockClassroomService = new Mock<IClassroomService>();
        _classroomController = new ClassroomsController(_mockClassroomService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var classrooms = new List<Classroom>
        {
            new Classroom { ClassId = 1, ClassName = "1.A" },
            new Classroom { ClassId = 2, ClassName = "1.B" },
        };
        _mockClassroomService
            .Setup(repo => repo.GetAllClassroomAsync())
            .ReturnsAsync(classrooms);

        // Act
        var result = await _classroomController.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResult()
    {
        // Arrange
        var createClassroom = new CreateClassroomDTO { ClassName = "1.A" };
        _mockClassroomService
            .Setup(repo => repo.InsertClassroomAsync(createClassroom))
            .ReturnsAsync(1);
        
        // Act
        var result = await _classroomController.Create(createClassroom);

        // Assert
        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequestResult()
    {
        // Arrange
        var createClassroom = new CreateClassroomDTO { ClassName = "1.A" };
        _mockClassroomService
            .Setup(repo => repo.InsertClassroomAsync(createClassroom))
            .ReturnsAsync(0);
        
        // Act
        var result = await _classroomController.Create(createClassroom);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOkObjectResult()
    {
        // Arrange
        var id =  1;
        var createClassroom = new CreateClassroomDTO { ClassName = "1.A" };
        _mockClassroomService
            .Setup(repo => repo.UpdateClassroomAsync(createClassroom, id))
            .ReturnsAsync(1);
        
        // Act
        var result = await _classroomController.Update(createClassroom, id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequestResult()
    {
        // Arrange
        var id =  1;
        var createClassroom = new CreateClassroomDTO { ClassName = "1.A" };
        _mockClassroomService
            .Setup(repo => repo.UpdateClassroomAsync(createClassroom, id))
            .ReturnsAsync(0);
        
        // Act
        var result = await _classroomController.Update(createClassroom, id);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOkObjectResult()
    {
        // Arrange
        var id =  1;
        _mockClassroomService
            .Setup(repo => repo.CheckChildrenInClassroom(id))
            .ReturnsAsync(false);
        _mockClassroomService
            .Setup(repo => repo.DeleteClassroomAsync(id))
            .ReturnsAsync(id);
        
        // Act
        var result = await _classroomController.Delete(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequestResult_WhenChildrenArePartOfClassroom()
    {
        // Arrange
        var id = 1;
        _mockClassroomService
            .Setup(repo => repo.CheckChildrenInClassroom(id))
            .ReturnsAsync(true);
        _mockClassroomService
            .Setup(repo => repo.DeleteClassroomAsync(id))
            .ReturnsAsync(id);
        
        // Act
        var result = await _classroomController.Delete(id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequestResult()
    {
        // Arrange
        var id =  1;
        _mockClassroomService
            .Setup(repo => repo.CheckChildrenInClassroom(id))
            .ReturnsAsync(false);
        _mockClassroomService
            .Setup(repo => repo.DeleteClassroomAsync(id))
            .ReturnsAsync(0);
        
        // Act
        var result = await _classroomController.Delete(id);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}