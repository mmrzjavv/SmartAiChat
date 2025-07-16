using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Application.Commands.AIConfiguration
{
    public class UpdateAiConfigurationCommand : IRequest<AiConfigurationDto>
    {
        public bool IsEnabled { get; set; }
        public AiProvider Provider { get; set; }
        public string ModelName { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiEndpoint { get; set; }
        public int MaxWordLimit { get; set; }
        public int MaxDailyMessages { get; set; }
        public decimal Temperature { get; set; }
        public int MaxTokens { get; set; }
        public string SystemPrompt { get; set; }
        public string? WelcomeMessage { get; set; }
        public string? FallbackMessage { get; set; }
        public bool UseKnowledgeBase { get; set; }
        public bool EnableContextHistory { get; set; }
        public int ContextHistoryLimit { get; set; }
        public bool EnableSentimentAnalysis { get; set; }
        public bool EnableAutoTranslation { get; set; }
        public string? SupportedLanguages { get; set; }
        public bool EnableHandoffToOperator { get; set; }
        public string? HandoffTriggerKeywords { get; set; }
        public int HandoffConfidenceThreshold { get; set; }
        public bool EnableFaqSuggestions { get; set; }
        public bool EnableTypingIndicator { get; set; }
        public int TypingIndicatorDelay { get; set; }
        public string? CustomInstructions { get; set; }
        public string? RestrictedTopics { get; set; }
        public bool LogConversations { get; set; }
        public bool EnableAnalytics { get; set; }
        public decimal InputCostPer1000Tokens { get; set; }
        public decimal OutputCostPer1000Tokens { get; set; }
    }
}
