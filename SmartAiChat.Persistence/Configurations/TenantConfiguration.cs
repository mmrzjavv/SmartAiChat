using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAiChat.Domain.Entities;

namespace SmartAiChat.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Domain)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Phone)
            .HasMaxLength(50);

        builder.Property(t => t.Address)
            .HasMaxLength(500);

        builder.Property(t => t.ContactPerson)
            .HasMaxLength(200);

        builder.Property(t => t.LogoUrl)
            .HasMaxLength(500);

        builder.Property(t => t.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(t => t.TimeZone)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("UTC");

        builder.Property(t => t.Language)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("en");

        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(t => t.Domain)
            .IsUnique()
            .HasDatabaseName("IX_Tenants_Domain");

        builder.HasIndex(t => t.Email)
            .IsUnique()
            .HasDatabaseName("IX_Tenants_Email");

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_Tenants_IsActive");

        // Relationships
        builder.HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.ChatSessions)
            .WithOne(cs => cs.Tenant)
            .HasForeignKey(cs => cs.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.TenantSubscription)
            .WithOne(ts => ts.Tenant)
            .HasForeignKey<TenantSubscription>(ts => ts.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.AiConfiguration)
            .WithOne(ac => ac.Tenant)
            .HasForeignKey<AiConfiguration>(ac => ac.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.FaqEntries)
            .WithOne(fe => fe.Tenant)
            .HasForeignKey(fe => fe.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.AiTrainingFiles)
            .WithOne(atf => atf.Tenant)
            .HasForeignKey(atf => atf.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 