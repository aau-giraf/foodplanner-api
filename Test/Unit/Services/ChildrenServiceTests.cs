using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerServices.Account;
using Moq;

namespace Test.Unit.Service;

public class ChildrenServiceTests 
{
    private readonly Mock<IChildrenRepository> _mockChildrenRepository;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ChildrenService _childrenService;

    public ChildrenServiceTests()
    {
        _mockChildrenRepository = new Mock<IChildrenRepository>();
        _mockAuthService = new Mock<IAuthService>();
        _mockUserRepository = new Mock<IUserRepository>();

        _childrenService = new ChildrenService(
            _mockChildrenRepository.Object,
            _mockUserRepository.Object,
            _mockAuthService.Object
        );
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllChildrenAsync_ReturnsAllChildren()
    {
        // Arrange
        var expectedChildren = new List<Children>
        {
            new Children { ChildId = 1, FirstName = "niels", LastName = "nielsen" },
            new Children { ChildId = 2, FirstName = "ole", LastName = "olsen" },
        };
        _mockChildrenRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedChildren);
                
        // Act
        var result = (await _childrenService.GetAllChildrenAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedChildren.Count, result.Count());
        Assert.All(result, Children => Assert.Contains(expectedChildren, c => 
                                                    c.ChildId == Children.ChildId && 
                                                    c.FirstName == Children.FirstName && 
                                                    c.LastName == Children.LastName));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateChildrenAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        var updateChildDto = new ChildrenDTO { ChildId = expectedId, FirstName = "frederik", LastName = "nielsen" };
        _mockChildrenRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Children>()))
            .ReturnsAsync(expectedId);

        
        // Act
        var result = await _childrenService.UpdateChildrenAsync(updateChildDto);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteChildrenAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        _mockChildrenRepository
            .Setup(repo => repo.DeleteAsync(expectedId))
            .ReturnsAsync(expectedId);
        
        // Act
        var result = await _childrenService.DeleteChildrenAsync(expectedId);

        // Assert
        Assert.Equal(expectedId, result);
    }
}