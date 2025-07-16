using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Application.DTOs;

public class AiConfigurationDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public bool IsEnabled { get; set; }
    public AiProvider Provider { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public int MaxWordLimit { get; set; }
    public int MaxDailyMessages { get; set; }
    public decimal Temperature { get; set; }
    public int MaxTokens { get; set; }
    public string SystemPrompt { get; set; } = string.Empty;
    public string? WelcomeMessage { get; set; }
    public string? FallbackMessage { get; set; }
    public bool UseKnowledgeBase { get; set; }
    public bool EnableContextHistory { get; set; }
    public int ContextHistoryLimit { get; set; }
    public bool EnableHandoffToOperator { get; set; }
    public bool EnableFaqSuggestions { get; set; }
    public bool EnableTypingIndicator { get; set; }
    public int TypingIndicatorDelay { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 