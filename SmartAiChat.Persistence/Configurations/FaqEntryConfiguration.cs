using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAiChat.Domain.Entities;

namespace SmartAiChat.Persistence.Configurations;

public class FaqEntryConfiguration : IEntityTypeConfiguration<FaqEntry>
{
    public void Configure(EntityTypeBuilder<FaqEntry> builder)
    {
        builder.ToTable("FaqEntries");

        builder.HasKey(fe => fe.Id);

        builder.Property(fe => fe.TenantId).IsRequired();
        builder.Property(fe => fe.Question).IsRequired().HasMaxLength(1000);
        builder.Property(fe => fe.Answer).IsRequired().HasMaxLength(4000);
        builder.Property(fe => fe.Category).HasMaxLength(200);
        builder.Property(fe => fe.Tags).HasColumnType("nvarchar(max)");
        builder.Property(fe => fe.IsActive).HasDefaultValue(true);
        builder.Property(fe => fe.SortOrder).HasDefaultValue(0);
        builder.Property(fe => fe.Language).IsRequired().HasMaxLength(10).HasDefaultValue("en");
        builder.Property(fe => fe.ViewCount).HasDefaultValue(0);
        builder.Property(fe => fe.UsefulCount).HasDefaultValue(0);
        builder.Property(fe => fe.NotUsefulCount).HasDefaultValue(0);
        builder.Property(fe => fe.LastUsedAt);
        builder.Property(fe => fe.CreatedByUserId);
        builder.Property(fe => fe.UpdatedByUserId);
        builder.Property(fe => fe.Keywords).HasMaxLength(1000);
        builder.Property(fe => fe.ConfidenceThreshold).HasPrecision(5, 2);
        builder.Property(fe => fe.EnableAutoSuggestion).HasDefaultValue(true);

        // Relationships
        builder.HasOne(fe => fe.Tenant)
            .WithMany(t => t.FaqEntries)
            .HasForeignKey(fe => fe.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(fe => fe.CreatedBy)
            .WithMany()
            .HasForeignKey(fe => fe.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(fe => fe.UpdatedBy)
            .WithMany()
            .HasForeignKey(fe => fe.UpdatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
} 