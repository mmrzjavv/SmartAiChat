using SmartAiChat.Shared;

namespace SmartAiChat.Domain.Entities;

public class AiTrainingFile : BaseEntity
{
    public Guid TenantId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileHash { get; set; } = string.Empty;
    public bool IsProcessed { get; set; } = false;
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessingStatus { get; set; }
    public string? ProcessingError { get; set; }
    public string? ExtractedText { get; set; }
    public int? WordCount { get; set; }
    public int? ChunkCount { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public string? Tags { get; set; } // JSON array
    public Guid UploadedByUserId { get; set; }
    public string? Metadata { get; set; } // JSON for additional processing info

    // Vector embedding properties (for future use)
    public string? EmbeddingModel { get; set; }
    public DateTime? EmbeddingCreatedAt { get; set; }
    public bool HasEmbeddings { get; set; } = false;

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual User UploadedBy { get; set; } = null!;
} 