using SmartAiChat.Shared;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid ChatSessionId { get; set; }
    public Guid? UserId { get; set; }  // Made UserId nullable

    public ChatMessageType MessageType { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsFromAi { get; set; } = false;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentName { get; set; }
    public string? AttachmentMimeType { get; set; }
    public long? AttachmentSize { get; set; }
    public string? Metadata { get; set; } // JSON for additional data
    public int? WordCount { get; set; }
    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAt { get; set; }
    public string? OriginalContent { get; set; }

    // AI-specific properties
    public string? AiModel { get; set; }
    public decimal? AiConfidenceScore { get; set; }
    public string? AiPromptTokens { get; set; }
    public string? AiResponseTokens { get; set; }
    public decimal? AiCost { get; set; }

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ChatSession ChatSession { get; set; } = null!;
}