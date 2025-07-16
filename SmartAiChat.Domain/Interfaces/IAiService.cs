using SmartAiChat.Domain.Entities;

namespace SmartAiChat.Domain.Interfaces;

public interface IAiService
{
    Task<string> GenerateResponseAsync(string prompt, AiConfiguration configuration, List<ChatMessage>? conversationHistory = null, CancellationToken cancellationToken = default);
    Task<bool> ValidateApiKeyAsync(string apiKey, string provider, CancellationToken cancellationToken = default);
    Task<decimal> CalculateCostAsync(string prompt, string response, AiConfiguration configuration, CancellationToken cancellationToken = default);
    Task<bool> ShouldHandoffToOperatorAsync(string message, AiConfiguration configuration, CancellationToken cancellationToken = default);
    Task<List<FaqEntry>> SuggestFaqsAsync(string message, List<FaqEntry> faqs, CancellationToken cancellationToken = default);
    Task<string> ExtractSentimentAsync(string message, CancellationToken cancellationToken = default);
    Task<string> TranslateMessageAsync(string message, string fromLanguage, string toLanguage, CancellationToken cancellationToken = default);
} 