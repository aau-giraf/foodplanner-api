using AutoMapper;
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
    private readonly Mock<IChatRepository> _mockChatRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IChildrenRepository> _mockChildrenRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IHubContext<ChatHub>> _mockHubContext;
    private readonly Mock<IHubClients> _mockHubClients;
    private readonly Mock<IClientProxy> _mockClientProxy;
    private readonly Mock<ILogger<ChatService>> _mockLogger;
    private readonly ChatService _chatService;

    public ChatServiceTests()
    {
        _mockChatRepository = new Mock<IChatRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockChildrenRepository = new Mock<IChildrenRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockHubContext = new Mock<IHubContext<ChatHub>>();
        _mockHubClients = new Mock<IHubClients>();
        _mockClientProxy = new Mock<IClientProxy>();
        _mockLogger = new Mock<ILogger<ChatService>>();

        _mockHubClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
        _mockHubContext.Setup(hub => hub.Clients).Returns(_mockHubClients.Object);

        _chatService = new ChatService(
            _mockChatRepository.Object,
            _mockMapper.Object,
            _mockChildrenRepository.Object,
            _mockUserRepository.Object,
            _mockHubContext.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task AddMessageAsync_InsertsMessageAndBroadcastsToCorrectGroup()
    {
        // Arrange
        var messageDto = new AddMessageDTO { ChatThreadId = 42, Content = "Hello" };
        const int userId = 7;
        var expectedGroupName = $"thread-{messageDto.ChatThreadId}";

        _mockMapper.Setup(m => m.Map<Message>(messageDto))
            .Returns(new Message { ChatThreadId = messageDto.ChatThreadId, Content = messageDto.Content });

        var broadcastDto = new UserNameFeedbackChatDTO { Content = messageDto.Content, FirstName = "" };
        _mockMapper.Setup(m => m.Map<UserNameFeedbackChatDTO>(It.IsAny<Message>()))
            .Returns(broadcastDto);

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(new User
            {
                Id = userId,
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                Password = "hashed",
                Role = UserRole.Parent,
                RoleApproved = true
            });

        // Act
        var result = await _chatService.AddMessageAsync(messageDto, userId);

        // Assert
        Assert.True(result);

        _mockChatRepository.Verify(repo => repo.InsertAsync(It.Is<Message>(m =>
            m.ChatThreadId == messageDto.ChatThreadId &&
            m.Content == messageDto.Content &&
            m.UserId == userId)), Times.Once);

        _mockHubClients.Verify(clients => clients.Group(expectedGroupName), Times.Once);

        _mockClientProxy.Verify(proxy => proxy.SendCoreAsync(
            "ReceiveMessage",
            It.Is<object[]>(args => args.Length == 1 && args[0] == broadcastDto),
            It.IsAny<CancellationToken>()), Times.Once);

        // The broadcast DTO is mutated in place with the sender's name looked up via IUserRepository,
        // matching the same first_name that GetMessagesAsync's SQL join would have provided.
        Assert.Equal("Alice", broadcastDto.FirstName);
    }

    [Fact]
    public async Task AddMessageAsync_BroadcastThrows_StillReturnsTrueAndInsertsMessage()
    {
        // Arrange
        var messageDto = new AddMessageDTO { ChatThreadId = 42, Content = "Hello" };
        const int userId = 7;

        _mockMapper.Setup(m => m.Map<Message>(messageDto))
            .Returns(new Message { ChatThreadId = messageDto.ChatThreadId, Content = messageDto.Content });

        _mockMapper.Setup(m => m.Map<UserNameFeedbackChatDTO>(It.IsAny<Message>()))
            .Returns(new UserNameFeedbackChatDTO { Content = messageDto.Content, FirstName = "" });

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        _mockClientProxy
            .Setup(proxy => proxy.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Hub is unreachable"));

        // Act
        var result = await _chatService.AddMessageAsync(messageDto, userId);

        // Assert — the broadcast failure must not propagate or change the outcome of message creation.
        Assert.True(result);

        _mockChatRepository.Verify(repo => repo.InsertAsync(It.Is<Message>(m =>
            m.ChatThreadId == messageDto.ChatThreadId &&
            m.Content == messageDto.Content &&
            m.UserId == userId)), Times.Once);
    }
}
