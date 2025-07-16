using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Persistence.Configurations;

public class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        builder.ToTable("TenantSubscriptions");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.TenantId)
            .IsRequired();

        builder.Property(ts => ts.TenantPlanId)
            .IsRequired();

        builder.Property(ts => ts.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(ts => ts.StartDate)
            .IsRequired();

        builder.Property(ts => ts.EndDate)
            .IsRequired();

        builder.Property(ts => ts.IsYearlyPlan)
            .HasDefaultValue(false);

        builder.Property(ts => ts.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ts => ts.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(ts => ts.AutoRenew)
            .HasDefaultValue(true);

        builder.Property(ts => ts.PaymentMethodId)
            .HasMaxLength(100);

        builder.Property(ts => ts.StripeSubscriptionId)
            .HasMaxLength(100);

        builder.Property(ts => ts.CancellationReason)
            .HasMaxLength(500);

        builder.Property(ts => ts.CurrentMonthAiMessages)
            .HasDefaultValue(0);

        builder.Property(ts => ts.CurrentDayAiMessages)
            .HasDefaultValue(0);

        builder.Property(ts => ts.LastUsageReset)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(ts => ts.TenantId)
            .IsUnique()
            .HasDatabaseName("IX_TenantSubscriptions_TenantId");

        builder.HasIndex(ts => ts.Status)
            .HasDatabaseName("IX_TenantSubscriptions_Status");

        builder.HasIndex(ts => ts.EndDate)
            .HasDatabaseName("IX_TenantSubscriptions_EndDate");

        // Relationships
        builder.HasOne(ts => ts.Tenant)
            .WithOne(t => t.TenantSubscription)
            .HasForeignKey<TenantSubscription>(ts => ts.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ts => ts.TenantPlan)
            .WithMany(tp => tp.TenantSubscriptions)
            .HasForeignKey(ts => ts.TenantPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 