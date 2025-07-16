namespace SmartAiChat.Application.DTOs;

public class AiTrainingFileDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessingStatus { get; set; }
    public string? ProcessingError { get; set; }
    public int? WordCount { get; set; }
    public int? ChunkCount { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool HasEmbeddings { get; set; }
    public DateTime CreatedAt { get; set; }
} 