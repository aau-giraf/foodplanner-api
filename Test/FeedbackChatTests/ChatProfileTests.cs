using AutoMapper;
using FoodplannerModels.FeedbackChat;

namespace Test.Service;

public class ChatProfileTests
{
    private readonly IMapper _mapper;

    public ChatProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ChatProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Map_MessageToUserNameFeedbackChatDTO_MapsMessageIdAndChatThreadIdByConvention()
    {
        // Arrange
        var message = new Message
        {
            MessageId = 123,
            ChatThreadId = 42,
            Content = "Hello",
            UserId = 7,
            Date = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<UserNameFeedbackChatDTO>(message);

        // Assert — AutoMapper's default convention (matching property names/types) is
        // expected to pick these up with no explicit .ForMember() configuration.
        Assert.Equal(123, dto.MessageId);
        Assert.Equal(42, dto.ChatThreadId);
    }
}
