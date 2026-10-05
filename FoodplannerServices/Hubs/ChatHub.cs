using System.Security.Claims;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// Adds the ChatHub to the FoodplannerServices.Hubs namespace
namespace FoodplannerServices.Hubs;

[Authorize(Roles = "Parent, Teacher")]
public class ChatHub : Hub
{
    private readonly IChatRepository _chatRepository;
    private readonly IChildrenRepository _childrenRepository;

    public ChatHub(IChatRepository chatRepository, IChildrenRepository childrenRepository)
    {
        _chatRepository = chatRepository;
        _childrenRepository = childrenRepository;
    }

    // Allows a user to join a chat thread group if they are authorized for that thread
    public async Task JoinThread(int chatThreadId)
    {
        await EnsureCallerIsAuthorizedForThreadAsync(chatThreadId);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"thread-{chatThreadId}");
    }

    // Allows a user to leave a chat thread group if they are authorized for that thread
    public async Task LeaveThread(int chatThreadId)
    {
        await EnsureCallerIsAuthorizedForThreadAsync(chatThreadId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"thread-{chatThreadId}");
    }

    // Ensures that the caller is authorized to access the specified chat thread
    private async Task EnsureCallerIsAuthorizedForThreadAsync(int chatThreadId)
    {
        // Retrieve chat thread id and user role
        var chatThread = await _chatRepository.GetChatThreadByIdAsync(chatThreadId);
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new HubException("Not authorized for this chat thread");
        }

        // Check if the user is a parent or teacher and if they are authorized for the chat thread
        var isAuthorized = false;
        if (Context.User != null && Context.User.IsInRole("Parent"))
        {
            var parents = await _childrenRepository.GetParentsByChildIdAsync(chatThread.ChildId);
            isAuthorized = parents.Any(parent => parent.Id == userId);
        }
        else if (Context.User != null && Context.User.IsInRole("Teacher"))
        {
            var teachers = await _childrenRepository.GetTeachersByChildIdAsync(chatThread.ChildId);
            isAuthorized = teachers.Any(teacher => teacher.Id == userId);
        }

        // If the user is not authorized for the chat thread, throw a HubException
        if (!isAuthorized)
        {
            throw new HubException("Not authorized for this chat thread");
        }
    }
}
