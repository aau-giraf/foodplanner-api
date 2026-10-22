using FoodplannerModels.FeedbackChat;
using FoodplannerModels.Account;
using FoodplannerServices.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FoodplannerServices.FeedbackChat;

public class ChatService(IChatRepository chatRepository, IChildrenRepository childrenRepository, IUserRepository userRepository, IHubContext<ChatHub> hubContext, ILogger<ChatService> logger) : IChatService
{
    // Adds a message to the database via AddMessageDTO and userId
    public async Task<bool> AddMessageAsync(AddMessageDTO addMessageDTO, int userId)
    {
        // Create temporary entity with necessary data for message creation
        var message = new AddMessageTemp
        {
            ChatThreadId = addMessageDTO.ChatThreadId,
            Content = addMessageDTO.Content,
            Date = System.DateTime.Now,
            UserId = userId
        };

        // Validate the user
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        } 

        // Insert the message into the database
        int messageId = await chatRepository.AddMessageAsync(message);
        try
        {
            // Create MessageDTO
            var messageDTO = new MessageDTO
            {
                MessageId = messageId,
                Content = message.Content,
                Date = message.Date,
                UserId = message.UserId,
                ChatThreadId = message.ChatThreadId,
                Archived = false,
                IsEdited = false,
                FirstName = user.FirstName
            };

            // Broadcast the message to the SignalR group for the chat thread
            await hubContext.Clients.Group($"thread-{message.ChatThreadId}").SendAsync("ReceiveMessage", messageDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to broadcast new FeedbackChat message to group thread-{message.ChatThreadId}");
        }
        return true;
    }

    // Gets all current messages for a chat thread
    public async Task<IEnumerable<MessageDTO>> GetMessagesAsync(int chatThreadId)
    {
        // Get all current Message entities
        IEnumerable<Message> messages = await chatRepository.GetMessagesByThreadIdAsync(chatThreadId);
        List<MessageDTO> messageDTOs = new();
        
        foreach (Message message in messages)
        {
            // Set message content to placeholder if its archived
            if (message.Archived)
            {
                message.Content = "Denne besked er blevet arkiveret.";
            }

            // Convert Message entity to MessageDTO for frontend
            var user = await userRepository.GetByIdAsync(message.UserId);
            var messageDTO = new MessageDTO {
                MessageId = message.MessageId,
                Content = message.Content,
                Date = message.Date,
                UserId = message.UserId,
                ChatThreadId = message.ChatThreadId,
                Archived = message.Archived,
                IsEdited = message.IsEdited,
                FirstName = user.FirstName
            };
            messageDTOs.Add(messageDTO);
        }
        return messageDTOs;
    }
    
    // Edit an existing message via a changed MessageDTO
    public async Task<bool> EditMessageAsync(EditMessageDTO message)
    {
        await chatRepository.EditMessageAsync(message);
        return true;
    }

    // Archive a message
    public async Task<bool> ArchiveMessageAsync(int messageId)
    {
        await chatRepository.ArchiveMessageAsync(messageId);
        return true;
    }
    
    // Get the chat thread ID for a child, creating a new chat thread if one does not exist.
    public async Task<int> GetThreadIdByChildIdAsync(int childId)
    {
        int? chatThreadId = await chatRepository.GetThreadIdByChildIdAsync(childId);
        if (chatThreadId == null)
        {
            return await chatRepository.CreateThreadByChildIdAsync(childId);
        }
        return chatThreadId.Value;
    }
    
    public async Task<IEnumerable<int>> GetThreadIdsByParentIdAsync(int UserId)
    {
        // Get all children for this parent
        IEnumerable<Children> children = await childrenRepository.GetChildrenByParentIdAsync(UserId);

        // Get list of chat thread ids from list of children
        List<int> chatThreadIds = new();
        foreach (Children child in children)
        {
            int threadId = await GetThreadIdByChildIdAsync(child.ChildId);
            chatThreadIds.Add(threadId);
        }
        return chatThreadIds;
    }
}