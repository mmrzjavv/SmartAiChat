using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Enums;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Commands.ChatMessages;

public class SendMessageCommand : IRequest<ApiResponse<ChatMessageDto>>
{
    public Guid ChatSessionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public ChatMessageType MessageType { get; set; } = ChatMessageType.CustomerMessage;
    public string? AttachmentUrl { get; set; }
    public string? AttachmentName { get; set; }
    public string? AttachmentMimeType { get; set; }
    public long? AttachmentSize { get; set; }
} 