using SmartAiChat.Shared;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Domain.Entities;

public class TenantSubscription : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid TenantPlanId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsYearlyPlan { get; set; } = false;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime? LastBillingDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public bool AutoRenew { get; set; } = true;
    public string? PaymentMethodId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Usage tracking
    public int CurrentMonthAiMessages { get; set; } = 0;
    public int CurrentDayAiMessages { get; set; } = 0;
    public DateTime LastUsageReset { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual TenantPlan TenantPlan { get; set; } = null!;
} 