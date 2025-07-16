using SmartAiChat.Shared;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Domain.Entities;

public class AiConfiguration : BaseEntity
{
    public Guid TenantId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public AiProvider Provider { get; set; } = AiProvider.OpenAI;
    public string ModelName { get; set; } = "gpt-3.5-turbo";
    public string? ApiKey { get; set; }
    public string? ApiEndpoint { get; set; }
    public int MaxWordLimit { get; set; } = 500;
    public int MaxDailyMessages { get; set; } = 1000;
    public decimal Temperature { get; set; } = 0.7m;
    public int MaxTokens { get; set; } = 1000;
    public string SystemPrompt { get; set; } = "You are a helpful customer service assistant.";
    public string? WelcomeMessage { get; set; }
    public string? FallbackMessage { get; set; }
    public bool UseKnowledgeBase { get; set; } = true;
    public bool EnableContextHistory { get; set; } = true;
    public int ContextHistoryLimit { get; set; } = 10;
    public bool EnableSentimentAnalysis { get; set; } = false;
    public bool EnableAutoTranslation { get; set; } = false;
    public string? SupportedLanguages { get; set; } // JSON array
    public bool EnableHandoffToOperator { get; set; } = true;
    public string? HandoffTriggerKeywords { get; set; } // JSON array
    public int HandoffConfidenceThreshold { get; set; } = 80;
    public bool EnableFaqSuggestions { get; set; } = true;
    public bool EnableTypingIndicator { get; set; } = true;
    public int TypingIndicatorDelay { get; set; } = 2000; // milliseconds
    public string? CustomInstructions { get; set; }
    public string? RestrictedTopics { get; set; } // JSON array
    public bool LogConversations { get; set; } = true;
    public bool EnableAnalytics { get; set; } = true;

    // Pricing
    public decimal InputCostPer1000Tokens { get; set; } = 0.0015m;
    public decimal OutputCostPer1000Tokens { get; set; } = 0.002m;

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
} 