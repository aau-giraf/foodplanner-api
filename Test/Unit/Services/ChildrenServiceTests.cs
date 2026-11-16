using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using FoodplannerServices.Account;
using Moq;
using Test.Builder;

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

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddParentToChildAsync_WithValidParentRole_ShouldSucceed()
    {
        // Arrange
        var userId = 1;
        var childId = 1;
        var parentUser = new UserBuilder()
            .WithRole(UserRole.Parent)
            .WithRoleApproved(true)
            .Build();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(parentUser);
        _mockChildrenRepository.Setup(repo => repo.AddUserToChildAsync(userId, childId))
            .ReturnsAsync(1);

        // Act
        var result = await _childrenService.AddParentToChildAsync(userId, childId);

        // Assert
        Assert.Equal(1, result);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddUserToChildAsync(userId, childId), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddParentToChildAsync_WithNonParentRole_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var childId = 1;
        var nonParentUser = new UserBuilder()
            .WithRole(UserRole.Teacher)
            .WithRoleApproved(true)
            .Build();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(nonParentUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _childrenService.AddParentToChildAsync(userId, childId));

        Assert.Equal("Kun brugere med rolle 'Parent' kan tilføjes som forældre", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddUserToChildAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddParentToChildAsync_WithUnapprovedRole_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var childId = 1;
        var unapprovedParentUser = new UserBuilder()
            .WithRole(UserRole.Parent)
            .WithRoleApproved(false)
            .Build();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(unapprovedParentUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _childrenService.AddParentToChildAsync(userId, childId));

        Assert.Equal("Brugerens rolle er ikke godkendt", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddUserToChildAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddParentToChildAsync_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var userId = 999;
        var childId = 1;

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _childrenService.AddParentToChildAsync(userId, childId));

        Assert.Equal("Bruger ikke fundet", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddUserToChildAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddParentToChildAsync_ChildCanHaveMultipleParents_ShouldSucceed()
    {
        // Arrange
        var childId = 1;
        var parent1Id = 1;
        var parent2Id = 2;
        var parent3Id = 3;

        var parent1 = new UserBuilder()
            .WithRole(UserRole.Parent)
            .WithRoleApproved(true)
            .WithFirstName("Parent1")
            .Build();

        var parent2 = new UserBuilder()
            .WithRole(UserRole.Parent)
            .WithRoleApproved(true)
            .WithFirstName("Parent2")
            .Build();

        var parent3 = new UserBuilder()
            .WithRole(UserRole.Parent)
            .WithRoleApproved(true)
            .WithFirstName("Parent3")
            .Build();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(parent1Id))
            .ReturnsAsync(parent1);
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(parent2Id))
            .ReturnsAsync(parent2);
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(parent3Id))
            .ReturnsAsync(parent3);

        _mockChildrenRepository.Setup(repo => repo.AddUserToChildAsync(It.IsAny<int>(), childId))
            .ReturnsAsync(1);

        // Act - Add multiple parents to the same child
        var result1 = await _childrenService.AddParentToChildAsync(parent1Id, childId);
        var result2 = await _childrenService.AddParentToChildAsync(parent2Id, childId);
        var result3 = await _childrenService.AddParentToChildAsync(parent3Id, childId);

        // Assert
        Assert.Equal(1, result1);
        Assert.Equal(1, result2);
        Assert.Equal(1, result3);

        // Verify that all three parents were added to the same child
        _mockChildrenRepository.Verify(repo => repo.AddUserToChildAsync(parent1Id, childId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddUserToChildAsync(parent2Id, childId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddUserToChildAsync(parent3Id, childId), Times.Once);

        // Verify that GetByIdAsync was called for each parent
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(parent1Id), Times.Once);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(parent2Id), Times.Once);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(parent3Id), Times.Once);
    }
}