using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Persistence.Configurations;

public class AiConfigurationConfiguration : IEntityTypeConfiguration<AiConfiguration>
{
    public void Configure(EntityTypeBuilder<AiConfiguration> builder)
    {
        builder.ToTable("AiConfigurations");

        builder.HasKey(ac => ac.Id);

        builder.Property(ac => ac.TenantId)
            .IsRequired();

        builder.Property(ac => ac.IsEnabled)
            .HasDefaultValue(true);

        builder.Property(ac => ac.Provider)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(AiProvider.OpenAI);

        builder.Property(ac => ac.ModelName)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("gpt-3.5-turbo");

        builder.Property(ac => ac.ApiKey)
            .HasMaxLength(500);

        builder.Property(ac => ac.ApiEndpoint)
            .HasMaxLength(500);

        builder.Property(ac => ac.MaxWordLimit)
            .HasDefaultValue(500);

        builder.Property(ac => ac.MaxDailyMessages)
            .HasDefaultValue(1000);

        builder.Property(ac => ac.Temperature)
            .HasColumnType("decimal(3,2)")
            .HasDefaultValue(0.7m);

        builder.Property(ac => ac.MaxTokens)
            .HasDefaultValue(1000);

        builder.Property(ac => ac.SystemPrompt)
            .IsRequired()
            .HasColumnType("nvarchar(max)")
            .HasDefaultValue("You are a helpful customer service assistant.");

        builder.Property(ac => ac.WelcomeMessage)
            .HasMaxLength(500);

        builder.Property(ac => ac.FallbackMessage)
            .HasMaxLength(500);

        builder.Property(ac => ac.UseKnowledgeBase)
            .HasDefaultValue(true);

        builder.Property(ac => ac.EnableContextHistory)
            .HasDefaultValue(true);

        builder.Property(ac => ac.ContextHistoryLimit)
            .HasDefaultValue(10);

        builder.Property(ac => ac.EnableSentimentAnalysis)
            .HasDefaultValue(false);

        builder.Property(ac => ac.EnableAutoTranslation)
            .HasDefaultValue(false);

        builder.Property(ac => ac.SupportedLanguages)
            .HasColumnType("nvarchar(max)");

        builder.Property(ac => ac.EnableHandoffToOperator)
            .HasDefaultValue(true);

        builder.Property(ac => ac.HandoffTriggerKeywords)
            .HasColumnType("nvarchar(max)");

        builder.Property(ac => ac.HandoffConfidenceThreshold)
            .HasDefaultValue(80);

        builder.Property(ac => ac.EnableFaqSuggestions)
            .HasDefaultValue(true);

        builder.Property(ac => ac.EnableTypingIndicator)
            .HasDefaultValue(true);

        builder.Property(ac => ac.TypingIndicatorDelay)
            .HasDefaultValue(2000);

        builder.Property(ac => ac.CustomInstructions)
            .HasColumnType("nvarchar(max)");

        builder.Property(ac => ac.RestrictedTopics)
            .HasColumnType("nvarchar(max)");

        builder.Property(ac => ac.LogConversations)
            .HasDefaultValue(true);

        builder.Property(ac => ac.EnableAnalytics)
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(ac => ac.TenantId)
            .IsUnique()
            .HasDatabaseName("IX_AiConfigurations_TenantId");

        builder.HasIndex(ac => ac.IsEnabled)
            .HasDatabaseName("IX_AiConfigurations_IsEnabled");

        // Relationships
        builder.HasOne(ac => ac.Tenant)
            .WithOne(t => t.AiConfiguration)
            .HasForeignKey<AiConfiguration>(ac => ac.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 