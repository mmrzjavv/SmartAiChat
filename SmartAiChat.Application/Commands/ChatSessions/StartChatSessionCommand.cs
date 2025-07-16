using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Commands.ChatSessions;

public class StartChatSessionCommand : IRequest<ApiResponse<ChatSessionDto>>
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? Department { get; set; }
    public string? CustomerIpAddress { get; set; }
    public string? CustomerUserAgent { get; set; }
} 