using FoodplannerApi.Controller;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Controller;

public class ChildrenControllerTest
{
    private readonly Mock<IChildrenService> _mockChildrenService;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly ChildrensController _childrenController;

    public ChildrenControllerTest()
    {
        _mockChildrenService = new Mock<IChildrenService>();
        _mockAuthService = new Mock<IAuthService>();

        _childrenController = new ChildrensController(
            _mockChildrenService.Object,
            _mockAuthService.Object
        );
    }

    [Fact]
    public async Task GetAllChildrenClassesAsync_ReturnsOkObjectResult()
    {
        // Arrange
        var children = new List<ChildrenGetAllDTO> {
            new ChildrenGetAllDTO { ChildId = 1, FirstName = "niels", LastName = "nielsen", ClassName = "1.A" },
            new ChildrenGetAllDTO { ChildId = 1, FirstName = "ole", LastName = "olsen", ClassName = "1.A" },
        };
        _mockChildrenService
            .Setup(repo => repo.GetAllChildrenClassesAsync())
            .ReturnsAsync(children);

        // Act
        var result = await _childrenController.GetAllChildrenClassesAsync();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var children = new List<Children>
        {
            new Children { ChildId = 1, FirstName = "niels", LastName = "nielsen" },
            new Children { ChildId = 2, FirstName = "ole", LastName = "olsen" },
        };
        _mockChildrenService
            .Setup(repo => repo.GetAllChildrenAsync())
            .ReturnsAsync(children);

        // Act
        var result = await _childrenController.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetChildrenByParentId_ReturnsOkObjectResult()
    {
        // Arrange
        var token = "jwtToken";
        var children = new Children { ChildId = 1, FirstName = "niels", LastName = "nielsen" };
        _mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(token))
            .Returns("1");
        _mockChildrenService
            .Setup(repo => repo.GetChildrenByIdAsync(1))
            .ReturnsAsync(children);

        // Act
        var result = await _childrenController.GetChildrenByParentId(token);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetChildrenByParentId_ReturnsBadRequestResult_WhenParentIdIsInvalid()
    {
        // Arrange
        var token = "jwtToken";
        var children = new Children { ChildId = 1, FirstName = "niels", LastName = "nielsen" };
        _mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(token))
            .Returns("invalid");
        _mockChildrenService
            .Setup(repo => repo.GetChildrenByIdAsync(1))
            .ReturnsAsync(children);

        // Act
        var result = await _childrenController.GetChildrenByParentId(token);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResult()
    {
        // Arrange
        var token = "jwtToken";
        var createChildren = new ChildrenCreateDTO { FirstName = "niels", LastName = "nielsen", classId = 1 };
        var parentId = 1;
        _mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(token))
            .Returns("" + parentId);
        _mockChildrenService
            .Setup(repo => repo.CreateChildrenAsync(It.Is<ChildrenCreateParentDTO>(dto =>
                dto.FirstName == createChildren.FirstName &&
                dto.LastName == createChildren.LastName &&
                dto.classId == createChildren.classId &&
                dto.parentId == parentId
            )))
            .ReturnsAsync(1);
        
        // Act
        var result = await _childrenController.Create(token, createChildren);

        // Assert
        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequestResult()
    {
        // Arrange
        var token = "jwtToken";
        var createChildren = new ChildrenCreateDTO { FirstName = "niels", LastName = "nielsen", classId = 1 };
        var parentId = 1;
        _mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(token))
            .Returns("" + parentId);
        _mockChildrenService
            .Setup(repo => repo.CreateChildrenAsync(It.Is<ChildrenCreateParentDTO>(dto =>
                dto.FirstName == createChildren.FirstName &&
                dto.LastName == createChildren.LastName &&
                dto.classId == createChildren.classId &&
                dto.parentId == parentId
            )))
            .ReturnsAsync(0);
        
        // Act
        var result = await _childrenController.Create(token, createChildren);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequestResult_WhenParentIdIsInvalid()
    {
        // Arrange
        var token = "jwtToken";
        var createChildren = new ChildrenCreateDTO { FirstName = "niels", LastName = "nielsen", classId = 1 };
        var resultChildren = new ChildrenCreateParentDTO { FirstName = createChildren.FirstName, LastName = createChildren.LastName, classId = createChildren.classId, parentId = 1};
        _mockAuthService
            .Setup(auth => auth.RetrieveIdFromJwtToken(token))
            .Returns("invalid");
        _mockChildrenService
            .Setup(repo => repo.CreateChildrenAsync(resultChildren))
            .ReturnsAsync(1);
        
        // Act
        var result = await _childrenController.Create(token, createChildren);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult()
    {
        // Arrange
        var id = 1;
        _mockChildrenService
            .Setup(repo => repo.DeleteChildrenAsync(id))
            .ReturnsAsync(id);

        // Act
        var result = await _childrenController.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundResult()
    {
        // Arrange
        var id = 1;
        _mockChildrenService
            .Setup(repo => repo.DeleteChildrenAsync(id))
            .ReturnsAsync(0);

        // Act
        var result = await _childrenController.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}