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
    private readonly ChildrenController _childrenController;

    public ChildrenControllerTest()
    {
        _mockChildrenService = new Mock<IChildrenService>();
        _mockAuthService = new Mock<IAuthService>();

        _childrenController = new ChildrenController(
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
            new ChildrenGetAllDTO { ChildId = 2, FirstName = "ole", LastName = "olsen", ClassName = "1.A" },
        };
        _mockChildrenService
            .Setup(repo => repo.GetAllChildrenClassesAsync())
            .ReturnsAsync(children);

        // Act
        var result = await _childrenController.GetAllChildrenClasses();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOkObjectResult()
    {
        // Arrange
        var childrenDto = new List<ChildrenDTO>
        {
            new ChildrenDTO { ChildId = 1, FirstName = "Niels", LastName = "Nielsen" },
            new ChildrenDTO { ChildId = 2, FirstName = "Ole", LastName = "Olesen" }
        };
        _mockChildrenService
            .Setup(repo => repo.GetAllChildrenAsync())
            .ReturnsAsync(childrenDto);

        // Act
        var result = await _childrenController.GetAllChildren();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}