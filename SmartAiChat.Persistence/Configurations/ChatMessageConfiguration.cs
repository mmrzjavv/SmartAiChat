using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Persistence.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");

        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.TenantId)
            .IsRequired();

        builder.Property(cm => cm.ChatSessionId)
            .IsRequired();

        builder.Property(cm => cm.MessageType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(cm => cm.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(cm => cm.IsFromAi)
            .HasDefaultValue(false);

        builder.Property(cm => cm.IsRead)
            .HasDefaultValue(false);

        builder.Property(cm => cm.AttachmentUrl)
            .HasMaxLength(500);

        builder.Property(cm => cm.AttachmentName)
            .HasMaxLength(200);

        builder.Property(cm => cm.AttachmentMimeType)
            .HasMaxLength(100);

        builder.Property(cm => cm.Metadata)
            .HasColumnType("nvarchar(max)");

        builder.Property(cm => cm.IsEdited)
            .HasDefaultValue(false);

        builder.Property(cm => cm.OriginalContent)
            .HasColumnType("nvarchar(max)");

        builder.Property(cm => cm.AiModel)
            .HasMaxLength(100);

        builder.Property(cm => cm.AiConfidenceScore)
            .HasColumnType("decimal(5,4)");

        builder.Property(cm => cm.AiPromptTokens)
            .HasMaxLength(50);

        builder.Property(cm => cm.AiResponseTokens)
            .HasMaxLength(50);

        builder.Property(cm => cm.AiCost)
            .HasColumnType("decimal(10,6)");

        // Indexes
        builder.HasIndex(cm => cm.TenantId)
            .HasDatabaseName("IX_ChatMessages_TenantId");

        builder.HasIndex(cm => cm.ChatSessionId)
            .HasDatabaseName("IX_ChatMessages_ChatSessionId");

        builder.HasIndex(cm => cm.UserId)
            .HasDatabaseName("IX_ChatMessages_UserId");

        builder.HasIndex(cm => cm.MessageType)
            .HasDatabaseName("IX_ChatMessages_MessageType");

        builder.HasIndex(cm => cm.IsFromAi)
            .HasDatabaseName("IX_ChatMessages_IsFromAi");

        builder.HasIndex(cm => cm.CreatedAt)
            .HasDatabaseName("IX_ChatMessages_CreatedAt");

        // Relationships
        builder.HasOne(cm => cm.Tenant)
            .WithMany()
            .HasForeignKey(cm => cm.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.ChatSession)
            .WithMany(cs => cs.Messages)
            .HasForeignKey(cm => cm.ChatSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.User)
            .WithMany(u => u.ChatMessages)
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
} 