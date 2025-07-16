using SmartAiChat.Shared;

namespace SmartAiChat.Domain.Entities;

public class OperatorActivity : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid OperatorId { get; set; }
    public Guid ChatSessionId { get; set; }
    public string Action { get; set; } = string.Empty; // "Joined", "Left", "Monitoring", "Transferred", etc.
    public string? Notes { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? Duration { get; set; } // in seconds
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Metadata { get; set; } // JSON for additional data

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual User Operator { get; set; } = null!;
    public virtual ChatSession ChatSession { get; set; } = null!;
} 