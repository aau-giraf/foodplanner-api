using Moq;
using Xunit;
using AutoMapper;
using FoodplannerModels.Account;
using FoodplannerServices.Account;
using FoodplannerApi.Helpers;
using Test.Builder;

namespace Test;

public class ChildrenServiceTests
{
    private readonly Mock<IChildrenRepository> _mockChildrenRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AuthService _authService;
    private readonly ChildrenService _childrenService;

    public ChildrenServiceTests()
    {
        _mockChildrenRepository = new Mock<IChildrenRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        
        // Create a real AuthService instance with a mock configuration
        var mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        _authService = new AuthService(mockConfiguration.Object);
        
        _childrenService = new ChildrenService(
            _mockChildrenRepository.Object,
            _mockUserRepository.Object,
            _mockMapper.Object,
            _authService
        );
    }

    [Fact]
    public async Task AddParentToChildAsync_WithValidParentRole_ShouldSucceed()
    {
        // Arrange
        var userId = 1;
        var childId = 1;
        var parentUser = new UserBuilder()
            .WithRole("Parent")
            .WithRoleApproved(true)
            .Build();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(parentUser);
        _mockChildrenRepository.Setup(repo => repo.AddParentToChildAsync(userId, childId))
            .ReturnsAsync(1);

        // Act
        var result = await _childrenService.AddParentToChildAsync(userId, childId);

        // Assert
        Assert.Equal(1, result);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddParentToChildAsync(userId, childId), Times.Once);
    }

    [Fact]
    public async Task AddParentToChildAsync_WithNonParentRole_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var childId = 1;
        var nonParentUser = new UserBuilder()
            .WithRole("Teacher")
            .WithRoleApproved(true)
            .Build();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(nonParentUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _childrenService.AddParentToChildAsync(userId, childId));

        Assert.Equal("Kun brugere med rolle 'Parent' kan tilføjes som forældre", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddParentToChildAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AddParentToChildAsync_WithUnapprovedRole_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var childId = 1;
        var unapprovedParentUser = new UserBuilder()
            .WithRole("Parent")
            .WithRoleApproved(false)
            .Build();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(unapprovedParentUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _childrenService.AddParentToChildAsync(userId, childId));

        Assert.Equal("Brugerens rolle er ikke godkendt", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddParentToChildAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AddParentToChildAsync_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var userId = 999;
        var childId = 1;

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((User)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _childrenService.AddParentToChildAsync(userId, childId));

        Assert.Equal("Bruger ikke fundet", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddParentToChildAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AddParentToChildAsync_ChildCanHaveMultipleParents_ShouldSucceed()
    {
        // Arrange
        var childId = 1;
        var parent1Id = 1;
        var parent2Id = 2;
        var parent3Id = 3;

        var parent1 = new UserBuilder()
            .WithRole("Parent")
            .WithRoleApproved(true)
            .WithFirstName("Parent1")
            .Build();

        var parent2 = new UserBuilder()
            .WithRole("Parent")
            .WithRoleApproved(true)
            .WithFirstName("Parent2")
            .Build();

        var parent3 = new UserBuilder()
            .WithRole("Parent")
            .WithRoleApproved(true)
            .WithFirstName("Parent3")
            .Build();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(parent1Id))
            .ReturnsAsync(parent1);
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(parent2Id))
            .ReturnsAsync(parent2);
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(parent3Id))
            .ReturnsAsync(parent3);

        _mockChildrenRepository.Setup(repo => repo.AddParentToChildAsync(It.IsAny<int>(), childId))
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
        _mockChildrenRepository.Verify(repo => repo.AddParentToChildAsync(parent1Id, childId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddParentToChildAsync(parent2Id, childId), Times.Once);
        _mockChildrenRepository.Verify(repo => repo.AddParentToChildAsync(parent3Id, childId), Times.Once);

        // Verify that GetByIdAsync was called for each parent
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(parent1Id), Times.Once);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(parent2Id), Times.Once);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(parent3Id), Times.Once);
    }
}
