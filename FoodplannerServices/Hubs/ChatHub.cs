using System.Security.Claims;
using FoodplannerModels.Account;
using FoodplannerModels.FeedbackChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

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

    public async Task JoinThread(int chatThreadId)
    {
        await EnsureCallerIsAuthorizedForThreadAsync(chatThreadId);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"thread-{chatThreadId}");
    }

    public async Task LeaveThread(int chatThreadId)
    {
        await EnsureCallerIsAuthorizedForThreadAsync(chatThreadId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"thread-{chatThreadId}");
    }

    private async Task EnsureCallerIsAuthorizedForThreadAsync(int chatThreadId)
    {
        var chatThread = await _chatRepository.GetChatThreadByIdAsync(chatThreadId);

        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new HubException("Not authorized for this chat thread");
        }

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

        if (!isAuthorized)
        {
            throw new HubException("Not authorized for this chat thread");
        }
    }
}
