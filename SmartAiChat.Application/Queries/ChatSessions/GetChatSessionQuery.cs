using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Queries.ChatSessions;

public class GetChatSessionQuery : IRequest<ApiResponse<ChatSessionDto>>
{
    public Guid Id { get; set; }
    
    public GetChatSessionQuery(Guid id)
    {
        Id = id;
    }
} 