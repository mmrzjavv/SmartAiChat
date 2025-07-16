using SmartAiChat.Shared;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Domain.Entities;

public class ChatSession : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? OperatorId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public ChatSessionStatus? Status { get; set; } = ChatSessionStatus.Active;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerIpAddress { get; set; }
    public string? CustomerUserAgent { get; set; }
    public string? Subject { get; set; }
    public string? Department { get; set; }
    public DateTime? OperatorJoinedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string? EndReason { get; set; }
    public decimal? CustomerSatisfactionRating { get; set; }
    public string? CustomerFeedback { get; set; }
    public int MessageCount { get; set; } = 0;
    public int AiMessageCount { get; set; } = 0;
    public string? Tags { get; set; } // JSON array of tags
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual User Customer { get; set; } = null!;
    public virtual User? Operator { get; set; }
    public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    public virtual ICollection<OperatorActivity> OperatorActivities { get; set; } = new List<OperatorActivity>();
}