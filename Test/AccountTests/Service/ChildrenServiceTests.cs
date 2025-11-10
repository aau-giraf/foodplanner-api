using AutoMapper;
using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerServices.Account;
using Moq;

namespace Test.Service;

public class ChildrenServiceTests 
{
    private readonly Mock<IChildrenRepository> _mockChildrenRepository;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ChildrenService _childrenService;

    public ChildrenServiceTests()
    {
        _mockChildrenRepository = new Mock<IChildrenRepository>();
        _mockAuthService = new Mock<IAuthService>();
        _mockMapper = new Mock<IMapper>();
        _mockUserRepository = new Mock<IUserRepository>();

        _childrenService = new ChildrenService(
            _mockChildrenRepository.Object,
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockAuthService.Object
        );
    }

    [Fact]
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
        var result = await _childrenService.GetAllChildrenAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedChildren.Count, result.Count());
        Assert.All(result, Children => Assert.Contains(expectedChildren, c => 
                                                    c.ChildId == Children.ChildId && 
                                                    c.FirstName == Children.FirstName && 
                                                    c.LastName == Children.LastName));
    }

    [Fact]
    public async Task UpdateChildrenAsync_UpdatesValueInRepository()
    {
        // Arrange
        var expectedId = 1;
        var createChildren = new Children { ChildId = expectedId, FirstName = "niels", LastName = "nielsen" };
        _mockChildrenRepository
            .Setup(repo => repo.UpdateAsync(createChildren))
            .ReturnsAsync(expectedId);
        
        // Act
        var createChildrenDto = _mockMapper.Object.Map<ChildrenDTO>(createChildren);
        var result = await _childrenService.UpdateChildrenAsync(createChildrenDto);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
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