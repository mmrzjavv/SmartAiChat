using SmartAiChat.Shared;

namespace SmartAiChat.Domain.Entities;

public class FaqEntry : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Tags { get; set; } // JSON array for easier searching
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public string Language { get; set; } = "en";
    public int ViewCount { get; set; } = 0;
    public int UsefulCount { get; set; } = 0;
    public int NotUsefulCount { get; set; } = 0;
    public DateTime? LastUsedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? UpdatedByUserId { get; set; }

    // AI matching properties
    public string? Keywords { get; set; } // For keyword matching
    public decimal? ConfidenceThreshold { get; set; } = 0.7m;
    public bool EnableAutoSuggestion { get; set; } = true;

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual User? CreatedBy { get; set; }
    public virtual User? UpdatedBy { get; set; }
} 