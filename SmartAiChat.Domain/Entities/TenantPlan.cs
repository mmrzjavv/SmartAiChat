using SmartAiChat.Shared;

namespace SmartAiChat.Domain.Entities;

public class TenantPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public int MaxUsers { get; set; }
    public int MaxConcurrentSessions { get; set; }
    public int MaxDailyAiMessages { get; set; }
    public int MaxMonthlyAiMessages { get; set; }
    public int MaxAiWordLimit { get; set; }
    public bool AllowCustomAiModels { get; set; } = false;
    public bool AllowFileUploads { get; set; } = true;
    public int MaxFileUploadSizeMB { get; set; } = 10;
    public bool AllowOperatorTransfer { get; set; } = true;
    public bool AllowChatHistory { get; set; } = true;
    public int ChatHistoryDays { get; set; } = 30;
    public bool AllowAnalytics { get; set; } = true;
    public bool PrioritySupport { get; set; } = false;
    public string? Features { get; set; } // JSON string for additional features

    // Navigation properties
    public virtual ICollection<TenantSubscription> TenantSubscriptions { get; set; } = new List<TenantSubscription>();
} 