using System.Security.Claims;
using FoodplannerModels.Account;
using FoodplannerModels.FeedbackChat;
using FoodplannerServices.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Test.Service;

public class ChatHubTests
{
    private readonly Mock<IChatRepository> _mockChatRepository;
    private readonly Mock<IChildrenRepository> _mockChildrenRepository;
    private readonly Mock<HubCallerContext> _mockContext;
    private readonly Mock<IGroupManager> _mockGroups;
    private readonly ChatHub _chatHub;

    public ChatHubTests()
    {
        _mockChatRepository = new Mock<IChatRepository>();
        _mockChildrenRepository = new Mock<IChildrenRepository>();
        _mockContext = new Mock<HubCallerContext>();
        _mockGroups = new Mock<IGroupManager>();

        _mockContext.Setup(c => c.ConnectionId).Returns("connection-1");

        _chatHub = new ChatHub(_mockChatRepository.Object, _mockChildrenRepository.Object)
        {
            Context = _mockContext.Object,
            Groups = _mockGroups.Object
        };
    }

    private void SetCallerIdentity(int userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth", ClaimTypes.Name, ClaimTypes.Role);
        _mockContext.Setup(c => c.User).Returns(new ClaimsPrincipal(identity));
    }

    private static User MakeUser(int id) => new User
    {
        Id = id,
        FirstName = "Test",
        LastName = "User",
        Email = $"user{id}@example.com",
        Password = "hashed",
        Role = UserRole.Parent,
        RoleApproved = true
    };

    [Fact]
    public async Task JoinThread_ParentAssociatedWithChild_AddsConnectionToGroup()
    {
        // Arrange
        const int chatThreadId = 5;
        const int childId = 10;
        const int userId = 42;
        SetCallerIdentity(userId, "Parent");

        _mockChatRepository.Setup(repo => repo.GetChatThreadByIdAsync(chatThreadId))
            .ReturnsAsync(new ChatThread { ChatThreadId = chatThreadId, ChildId = childId });

        _mockChildrenRepository.Setup(repo => repo.GetParentsByChildIdAsync(childId))
            .ReturnsAsync(new List<User> { MakeUser(userId) });

        // Act
        await _chatHub.JoinThread(chatThreadId);

        // Assert
        _mockGroups.Verify(g => g.AddToGroupAsync("connection-1", $"thread-{chatThreadId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinThread_TeacherAssociatedWithChild_AddsConnectionToGroup()
    {
        // Arrange
        const int chatThreadId = 5;
        const int childId = 10;
        const int userId = 42;
        SetCallerIdentity(userId, "Teacher");

        _mockChatRepository.Setup(repo => repo.GetChatThreadByIdAsync(chatThreadId))
            .ReturnsAsync(new ChatThread { ChatThreadId = chatThreadId, ChildId = childId });

        _mockChildrenRepository.Setup(repo => repo.GetTeachersByChildIdAsync(childId))
            .ReturnsAsync(new List<User> { MakeUser(userId) });

        // Act
        await _chatHub.JoinThread(chatThreadId);

        // Assert
        _mockGroups.Verify(g => g.AddToGroupAsync("connection-1", $"thread-{chatThreadId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinThread_ParentNotAssociatedWithChild_ThrowsHubExceptionAndDoesNotJoinGroup()
    {
        // Arrange
        const int chatThreadId = 5;
        const int childId = 10;
        const int userId = 42;
        SetCallerIdentity(userId, "Parent");

        _mockChatRepository.Setup(repo => repo.GetChatThreadByIdAsync(chatThreadId))
            .ReturnsAsync(new ChatThread { ChatThreadId = chatThreadId, ChildId = childId });

        // The caller is a Parent, but not one of the parents actually linked to this child.
        _mockChildrenRepository.Setup(repo => repo.GetParentsByChildIdAsync(childId))
            .ReturnsAsync(new List<User> { MakeUser(999) });

        // Act & Assert
        await Assert.ThrowsAsync<HubException>(() => _chatHub.JoinThread(chatThreadId));

        _mockGroups.Verify(g => g.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LeaveThread_ParentNotAssociatedWithChild_ThrowsHubExceptionAndDoesNotLeaveGroup()
    {
        // Arrange
        const int chatThreadId = 5;
        const int childId = 10;
        const int userId = 42;
        SetCallerIdentity(userId, "Parent");

        _mockChatRepository.Setup(repo => repo.GetChatThreadByIdAsync(chatThreadId))
            .ReturnsAsync(new ChatThread { ChatThreadId = chatThreadId, ChildId = childId });

        _mockChildrenRepository.Setup(repo => repo.GetParentsByChildIdAsync(childId))
            .ReturnsAsync(new List<User> { MakeUser(999) });

        // Act & Assert
        await Assert.ThrowsAsync<HubException>(() => _chatHub.LeaveThread(chatThreadId));

        _mockGroups.Verify(g => g.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
