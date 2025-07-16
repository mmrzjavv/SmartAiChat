using SmartAiChat.Shared;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public string Currency { get; set; } = "USD";

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    public virtual TenantSubscription? TenantSubscription { get; set; }
    public virtual AiConfiguration? AiConfiguration { get; set; }
    public virtual ICollection<FaqEntry> FaqEntries { get; set; } = new List<FaqEntry>();
    public virtual ICollection<AiTrainingFile> AiTrainingFiles { get; set; } = new List<AiTrainingFile>();
} 