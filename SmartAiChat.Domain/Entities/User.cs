using SmartAiChat.Shared;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Domain.Entities;

public class User : BaseEntity
{
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public string? Avatar { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public string Language { get; set; } = "en";
    public string TimeZone { get; set; } = "UTC";

    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    // public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public virtual ICollection<OperatorActivity> OperatorActivities { get; set; } = new List<OperatorActivity>();
}