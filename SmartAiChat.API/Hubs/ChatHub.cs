using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SmartAiChat.Application.Commands.ChatMessages;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Enums;
using System;
using System.Threading.Tasks;

namespace SmartAiChat.API.Hubs;

public class ChatHub : Hub
{
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<ChatHub> _logger;
    private readonly ISender _sender;
    private static readonly Dictionary<string, string> _connections = new Dictionary<string, string>();

    private readonly IUnitOfWork _unitOfWork;

    public ChatHub(ITenantContext tenantContext, ILogger<ChatHub> logger, ISender sender, IUnitOfWork unitOfWork)
    {
        _tenantContext = tenantContext;
        _logger = logger;
        _sender = sender;
        _unitOfWork = unitOfWork;
    }

    public async Task JoinChatSession(string sessionId)
    {
        try
        {
            var tenantId = _tenantContext.GetTenantId();
            var chatSession = await _unitOfWork.ChatSessions.GetByIdAsync(Guid.Parse(sessionId), tenantId);

            if (chatSession == null)
            {
                await Clients.Caller.SendAsync("Error", "Chat session not found or you do not have permission to access it.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
            _connections[Context.ConnectionId] = sessionId;
            await Clients.Group($"chat_{sessionId}").SendAsync("UserJoined", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining chat session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("Error", "An error occurred while joining the chat session.");
        }
    }

    public async Task LeaveChatSession(string sessionId)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
            await Clients.Group($"chat_{sessionId}").SendAsync("UserLeft", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving chat session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("Error", "An error occurred while leaving the chat session.");
        }
    }

    public async Task SendMessageToSession(string sessionId, string message, string messageType)
    {
        try
        {
            if (!Enum.TryParse<ChatMessageType>(messageType, true, out var chatMessageType))
            {
                throw new ArgumentException("Invalid message type.", nameof(messageType));
            }

            var command = new SendMessageCommand
            {
                ChatSessionId = Guid.Parse(sessionId),
                Content = message,
                MessageType = chatMessageType
            };

            var result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                await Clients.Group($"chat_{sessionId}").SendAsync("ReceiveMessage", result.Data);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("Error", "An error occurred while sending the message.");
        }
    }

    public async Task OperatorJoinSession(string sessionId)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
            await Clients.Group($"chat_{sessionId}").SendAsync("OperatorJoined", new
            {
                SessionId = sessionId,
                OperatorId = Context.ConnectionId,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining operator to session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("Error", "An error occurred while joining the operator to the session.");
        }
    }

    public async Task StartTyping(string sessionId)
    {
        try
        {
            await Clients.OthersInGroup($"chat_{sessionId}").SendAsync("UserStartedTyping", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending start typing notification to session {SessionId}", sessionId);
        }
    }

    public async Task StopTyping(string sessionId)
    {
        try
        {
            await Clients.OthersInGroup($"chat_{sessionId}").SendAsync("UserStoppedTyping", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending stop typing notification to session {SessionId}", sessionId);
        }
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_connections.TryGetValue(Context.ConnectionId, out var sessionId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
            _connections.Remove(Context.ConnectionId);
            await Clients.Group($"chat_{sessionId}").SendAsync("UserLeft", Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }
} 