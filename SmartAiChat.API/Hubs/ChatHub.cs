using Microsoft.AspNetCore.SignalR;
using SmartAiChat.Domain.Interfaces;

namespace SmartAiChat.API.Hubs;

public class ChatHub : Hub
{
    private readonly ITenantContext _tenantContext;

    public ChatHub(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public async Task JoinChatSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
        await Clients.Group($"chat_{sessionId}").SendAsync("UserJoined", Context.ConnectionId);
    }

    public async Task LeaveChatSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
        await Clients.Group($"chat_{sessionId}").SendAsync("UserLeft", Context.ConnectionId);
    }

    public async Task SendMessageToSession(string sessionId, string message, string messageType)
    {
        await Clients.Group($"chat_{sessionId}").SendAsync("ReceiveMessage", new
        {
            SessionId = sessionId,
            Message = message,
            MessageType = messageType,
            Timestamp = DateTime.UtcNow,
            ConnectionId = Context.ConnectionId
        });
    }

    public async Task OperatorJoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
        await Clients.Group($"chat_{sessionId}").SendAsync("OperatorJoined", new
        {
            SessionId = sessionId,
            OperatorId = Context.ConnectionId,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task StartTyping(string sessionId)
    {
        await Clients.OthersInGroup($"chat_{sessionId}").SendAsync("UserStartedTyping", Context.ConnectionId);
    }

    public async Task StopTyping(string sessionId)
    {
        await Clients.OthersInGroup($"chat_{sessionId}").SendAsync("UserStoppedTyping", Context.ConnectionId);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
} 