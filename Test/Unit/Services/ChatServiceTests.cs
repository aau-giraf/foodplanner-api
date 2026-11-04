using FoodplannerModels.Account;
using FoodplannerModels.FeedbackChat;
using FoodplannerServices.FeedbackChat;
using FoodplannerServices.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.Service;

public class ChatServiceTests
{
    // Setup mocked repositories and services
    private readonly Mock<IChatRepository> mockChatRepository;
    private readonly Mock<IChildrenRepository> mockChildrenRepository;
    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly Mock<IHubContext<ChatHub>> mockHubContext;
    private readonly Mock<IHubClients> mockHubClients;
    private readonly Mock<IClientProxy> mockClientProxy;
    private readonly Mock<ILogger<ChatService>> mockLogger;
    private readonly ChatService chatService;
    public ChatServiceTests()
    {
        mockChatRepository = new Mock<IChatRepository>();
        mockChildrenRepository = new Mock<IChildrenRepository>();
        mockUserRepository = new Mock<IUserRepository>();
        mockHubContext = new Mock<IHubContext<ChatHub>>();
        mockHubClients = new Mock<IHubClients>();
        mockClientProxy = new Mock<IClientProxy>();
        mockLogger = new Mock<ILogger<ChatService>>();

        mockHubClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);
        mockHubContext.Setup(hub => hub.Clients).Returns(mockHubClients.Object);

        chatService = new ChatService(
            mockChatRepository.Object,
            mockChildrenRepository.Object,
            mockUserRepository.Object,
            mockHubContext.Object,
            mockLogger.Object
        );
    }

    // Helper function for creating a user
    private static User CreateUser(int userId)
    {
        return new User
        {
            Id = userId,
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            Password = "hashed",
            Role = UserRole.Parent,
            RoleApproved = true
        };
    }

    [Fact]
    public async Task AddMessageAsync_Success()
    {
        // Arrange
        var addMessageDTO = new AddMessageDTO
        {
            ChatThreadId = 42,
            Content = "Hello"
        };
        const int userId = 7;
        const int messageId = 123;
        var expectedGroupName = $"thread-{addMessageDTO.ChatThreadId}";
        mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(CreateUser(userId));
        mockChatRepository.Setup(repo => repo.AddMessageAsync(It.IsAny<AddMessageTemp>())).ReturnsAsync(messageId);

        // Act
        var result = await chatService.AddMessageAsync(addMessageDTO, userId);

        // Assert
        Assert.True(result);
        mockChatRepository.Verify(
            repo => repo.AddMessageAsync(It.Is<AddMessageTemp>(message =>
                message.ChatThreadId == addMessageDTO.ChatThreadId &&
                message.Content == addMessageDTO.Content &&
                message.UserId == userId)),
            Times.Once);
        mockHubClients.Verify(clients => clients.Group(expectedGroupName),Times.Once);

        mockClientProxy.Verify(
            proxy => proxy.SendCoreAsync(
                "ReceiveMessage",
                It.Is<object?[]>(args =>
                    args.Length == 1 &&
                    args[0] is MessageDTO &&
                    ((MessageDTO)args[0]!).MessageId == messageId &&
                    ((MessageDTO)args[0]!).ChatThreadId == addMessageDTO.ChatThreadId &&
                    ((MessageDTO)args[0]!).Content == addMessageDTO.Content &&
                    ((MessageDTO)args[0]!).UserId == userId &&
                    ((MessageDTO)args[0]!).FirstName == "Alice" &&
                    !((MessageDTO)args[0]!).Archived &&
                    !((MessageDTO)args[0]!).IsEdited),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task AddMessageAsync_UserNotFound()
    {
        // Arrange
        var addMessageDTO = new AddMessageDTO
        {
            ChatThreadId = 42,
            Content = "Hello"
        };
        const int userId = 999;
        mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await chatService.AddMessageAsync(addMessageDTO, userId);

        // Assert
        Assert.False(result);
        mockChatRepository.Verify(repo => repo.AddMessageAsync(It.IsAny<AddMessageTemp>()),Times.Never);
        mockClientProxy.Verify(
            proxy => proxy.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AddMessageAsync_BroadcastThrows()
    {
        // Arrange
        var addMessageDTO = new AddMessageDTO
        {
            ChatThreadId = 42,
            Content = "Hello"
        };
        const int userId = 7;
        const int messageId = 123;
        mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(CreateUser(userId));
        mockChatRepository.Setup(repo => repo.AddMessageAsync(It.IsAny<AddMessageTemp>())).ReturnsAsync(messageId);
        mockClientProxy
            .Setup(proxy => proxy.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Hub is unreachable"));

        // Act
        var result = await chatService.AddMessageAsync(addMessageDTO, userId);

        // Assert
        Assert.True(result);
        mockChatRepository.Verify(
            repo => repo.AddMessageAsync(It.Is<AddMessageTemp>(message =>
                message.ChatThreadId == addMessageDTO.ChatThreadId &&
                message.Content == addMessageDTO.Content &&
                message.UserId == userId)),
            Times.Once);
    }

    [Fact]
    public async Task GetMessagesAsync_ReturnsMessagesIncludingMessageIdAndChatThreadId()
    {
        // Arrange
        const int chatThreadId = 42;
        const int userId = 7;

        var storedMessages = new List<Message>
        {
            new Message
            {
                MessageId = 1,
                Date = DateTime.Today,
                ChatThreadId = chatThreadId,
                Content = "Hi",
                UserId = userId,
                Archived = false,
                IsEdited = false
            }
        };

        mockChatRepository
            .Setup(repo => repo.GetMessagesByThreadIdAsync(chatThreadId))
            .ReturnsAsync(storedMessages);

        mockUserRepository
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(CreateUser(userId));

        // Act
        var result = (await chatService.GetMessagesAsync(chatThreadId)).ToList();

        // Assert
        var dto = Assert.Single(result);

        Assert.Equal(1, dto.MessageId);
        Assert.Equal(chatThreadId, dto.ChatThreadId);
        Assert.Equal("Hi", dto.Content);
        Assert.Equal(userId, dto.UserId);
        Assert.Equal("Alice", dto.FirstName);
        Assert.False(dto.Archived);
        Assert.False(dto.IsEdited);
    }
}