using FoodplannerApi.Controller;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Unit.Controller;

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
    [Trait("Category", "Unit")]
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