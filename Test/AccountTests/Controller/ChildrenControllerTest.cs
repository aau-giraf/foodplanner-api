using FoodplannerApi.Controller;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Test.Builder;

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
            new ChildrenGetAllDTO { ChildId = 2, FirstName = "ole", LastName = "olsen", ClassName = "1.A" },
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
        var childrenDtos = new List<ChildrenDTO>
        {
            new ChildBuilder().WithChildId(1).WithFirstName("Niels").WithLastName("Nielsen").Build(),
            new ChildBuilder().WithChildId(2).WithFirstName("Ole").WithLastName("Olesen").Build()
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