using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAiChat.Domain.Entities;

namespace SmartAiChat.Persistence.Configurations;

public class TenantPlanConfiguration : IEntityTypeConfiguration<TenantPlan>
{
    public void Configure(EntityTypeBuilder<TenantPlan> builder)
    {
        builder.ToTable("TenantPlans");

        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(tp => tp.Description)
            .HasMaxLength(1000);

        builder.Property(tp => tp.MonthlyPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(tp => tp.YearlyPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(tp => tp.IsActive)
            .HasDefaultValue(true);

        builder.Property(tp => tp.MaxUsers)
            .IsRequired();

        builder.Property(tp => tp.MaxConcurrentSessions)
            .IsRequired();

        builder.Property(tp => tp.MaxDailyAiMessages)
            .IsRequired();

        builder.Property(tp => tp.MaxMonthlyAiMessages)
            .IsRequired();

        builder.Property(tp => tp.MaxAiWordLimit)
            .IsRequired();

        builder.Property(tp => tp.AllowCustomAiModels)
            .HasDefaultValue(false);

        builder.Property(tp => tp.AllowFileUploads)
            .HasDefaultValue(true);

        builder.Property(tp => tp.MaxFileUploadSizeMB)
            .HasDefaultValue(10);

        builder.Property(tp => tp.AllowOperatorTransfer)
            .HasDefaultValue(true);

        builder.Property(tp => tp.AllowChatHistory)
            .HasDefaultValue(true);

        builder.Property(tp => tp.ChatHistoryDays)
            .HasDefaultValue(30);

        builder.Property(tp => tp.AllowAnalytics)
            .HasDefaultValue(true);

        builder.Property(tp => tp.PrioritySupport)
            .HasDefaultValue(false);

        builder.Property(tp => tp.Features)
            .HasColumnType("nvarchar(max)");

        // Indexes
        builder.HasIndex(tp => tp.Name)
            .IsUnique()
            .HasDatabaseName("IX_TenantPlans_Name");

        builder.HasIndex(tp => tp.IsActive)
            .HasDatabaseName("IX_TenantPlans_IsActive");

        // Relationships
        builder.HasMany(tp => tp.TenantSubscriptions)
            .WithOne(ts => ts.TenantPlan)
            .HasForeignKey(ts => ts.TenantPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 