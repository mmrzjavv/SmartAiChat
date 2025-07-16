using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Application.DTOs;

public class TenantSubscriptionDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid TenantPlanId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsYearlyPlan { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime? LastBillingDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public bool AutoRenew { get; set; }
    public int CurrentMonthAiMessages { get; set; }
    public int CurrentDayAiMessages { get; set; }
    public DateTime LastUsageReset { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public TenantPlanDto? TenantPlan { get; set; }
} 