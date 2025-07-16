using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Persistence.Configurations;

public class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.ToTable("ChatSessions");

        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.TenantId)
            .IsRequired();

        builder.Property(cs => cs.CustomerId)
            .IsRequired();

        builder.Property(cs => cs.SessionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cs => cs.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ChatSessionStatus.Active);

        builder.Property(cs => cs.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cs => cs.CustomerEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cs => cs.CustomerIpAddress)
            .HasMaxLength(45); // IPv6 support

        builder.Property(cs => cs.CustomerUserAgent)
            .HasMaxLength(500);

        builder.Property(cs => cs.Subject)
            .HasMaxLength(200);

        builder.Property(cs => cs.Department)
            .HasMaxLength(100);

        builder.Property(cs => cs.EndReason)
            .HasMaxLength(200);

        builder.Property(cs => cs.CustomerSatisfactionRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(cs => cs.CustomerFeedback)
            .HasMaxLength(1000);

        builder.Property(cs => cs.MessageCount)
            .HasDefaultValue(0);

        builder.Property(cs => cs.AiMessageCount)
            .HasDefaultValue(0);

        builder.Property(cs => cs.Tags)
            .HasColumnType("nvarchar(max)");

        builder.Property(cs => cs.Notes)
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(cs => cs.TenantId)
            .HasDatabaseName("IX_ChatSessions_TenantId");

        builder.HasIndex(cs => cs.SessionId)
            .IsUnique()
            .HasDatabaseName("IX_ChatSessions_SessionId");

        builder.HasIndex(cs => cs.Status)
            .HasDatabaseName("IX_ChatSessions_Status");

        builder.HasIndex(cs => cs.CustomerId)
            .HasDatabaseName("IX_ChatSessions_CustomerId");

        builder.HasIndex(cs => cs.OperatorId)
            .HasDatabaseName("IX_ChatSessions_OperatorId");

        builder.HasIndex(cs => cs.CreatedAt)
            .HasDatabaseName("IX_ChatSessions_CreatedAt");

        // Relationships
        builder.HasOne(cs => cs.Tenant)
            .WithMany(t => t.ChatSessions)
            .HasForeignKey(cs => cs.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cs => cs.Customer)
            .WithMany(u => u.ChatSessions)
            .HasForeignKey(cs => cs.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cs => cs.Operator)
            .WithMany()
            .HasForeignKey(cs => cs.OperatorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(cs => cs.Messages)
            .WithOne(cm => cm.ChatSession)
            .HasForeignKey(cm => cm.ChatSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cs => cs.OperatorActivities)
            .WithOne(oa => oa.ChatSession)
            .HasForeignKey(oa => oa.ChatSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 