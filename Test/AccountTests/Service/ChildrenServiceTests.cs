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
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();

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
        
        _mockMapper.Setup(m => m.Map<ChildrenDTO>(It.IsAny<Children>()))
            .Returns((Children src) => new ChildrenDTO
            {
                ChildId = src.ChildId,
                FirstName = src.FirstName,
                LastName = src.LastName
            });
                
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
        _mockChildrenRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Children>()))
            .ReturnsAsync(expectedId);
        
        _mockMapper.Setup(m => m.Map<Children>(It.IsAny<ChildrenDTO>()))
            .Returns((ChildrenDTO src) => new Children
            {
                ChildId = src.ChildId,
                FirstName = src.FirstName,
                LastName = src.LastName
            });
        var updateChildDto = new ChildrenDTO() { ChildId = expectedId, FirstName = "frederik", LastName = "nielsen" };
        
        // Act
        var result = await _childrenService.UpdateChildrenAsync(updateChildDto);

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