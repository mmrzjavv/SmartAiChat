namespace SmartAiChat.Application.DTOs;

public class FaqEntryDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string Language { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int UsefulCount { get; set; }
    public int NotUsefulCount { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public bool EnableAutoSuggestion { get; set; }
    public DateTime CreatedAt { get; set; }
} 