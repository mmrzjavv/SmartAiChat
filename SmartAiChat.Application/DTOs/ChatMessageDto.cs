using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Application.DTOs;

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ChatSessionId { get; set; }
    public Guid? UserId { get; set; }
    public ChatMessageType MessageType { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsFromAi { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentName { get; set; }
    public string? AttachmentMimeType { get; set; }
    public long? AttachmentSize { get; set; }
    public int? WordCount { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // AI-specific properties
    public string? AiModel { get; set; }
    public decimal? AiConfidenceScore { get; set; }
    
    // Navigation properties
    public UserDto? User { get; set; }
} 