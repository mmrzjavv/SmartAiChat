namespace SmartAiChat.Application.DTOs;

public class TenantPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public bool IsActive { get; set; }
    public int MaxUsers { get; set; }
    public int MaxConcurrentSessions { get; set; }
    public int MaxDailyAiMessages { get; set; }
    public int MaxMonthlyAiMessages { get; set; }
    public int MaxAiWordLimit { get; set; }
    public bool AllowCustomAiModels { get; set; }
    public bool AllowFileUploads { get; set; }
    public int MaxFileUploadSizeMB { get; set; }
    public bool AllowOperatorTransfer { get; set; }
    public bool AllowChatHistory { get; set; }
    public int ChatHistoryDays { get; set; }
    public bool AllowAnalytics { get; set; }
    public bool PrioritySupport { get; set; }
    public DateTime CreatedAt { get; set; }
} 