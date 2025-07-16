using Microsoft.Extensions.Logging;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using System.Text;
using System.Text.Json;

namespace SmartAiChat.Infrastructure.Services;

public class OpenAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAiService> _logger;

    public OpenAiService(HttpClient httpClient, ILogger<OpenAiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GenerateResponseAsync(string prompt, AiConfiguration configuration, List<ChatMessage>? conversationHistory = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!configuration.IsEnabled)
            {
                return configuration.FallbackMessage ?? "AI is currently disabled. Please contact support.";
            }

            // Build the conversation context
            var messages = new List<object>();
            
            // Add system prompt
            messages.Add(new { role = "system", content = configuration.SystemPrompt });

            // Add conversation history if enabled
            if (configuration.EnableContextHistory && conversationHistory != null)
            {
                var recentMessages = conversationHistory
                    .OrderBy(m => m.CreatedAt)
                    .TakeLast(configuration.ContextHistoryLimit)
                    .ToList();

                foreach (var msg in recentMessages)
                {
                    messages.Add(new 
                    { 
                        role = msg.IsFromAi ? "assistant" : "user", 
                        content = msg.Content 
                    });
                }
            }

            // Add current prompt
            messages.Add(new { role = "user", content = prompt });

            // Prepare the request
            var requestBody = new
            {
                model = configuration.ModelName,
                messages = messages,
                max_tokens = configuration.MaxTokens,
                temperature = (float)configuration.Temperature
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Set authorization header
            if (!string.IsNullOrEmpty(configuration.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", configuration.ApiKey);
            }

            // Make the API call
            var endpoint = configuration.ApiEndpoint ?? "https://api.openai.com/v1/chat/completions";
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var openAiResponse = JsonSerializer.Deserialize<OpenAiChatResponse>(responseContent);
                
                var assistantMessage = openAiResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? 
                    configuration.FallbackMessage ?? "I'm sorry, I couldn't generate a response.";

                // Check word limit
                if (configuration.MaxWordLimit > 0)
                {
                    var words = assistantMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length > configuration.MaxWordLimit)
                    {
                        assistantMessage = string.Join(" ", words.Take(configuration.MaxWordLimit)) + "...";
                    }
                }

                return assistantMessage;
            }
            else
            {
                _logger.LogError("OpenAI API call failed with status: {StatusCode}", response.StatusCode);
                return configuration.FallbackMessage ?? "I'm experiencing technical difficulties. Please try again later.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenAI API");
            return configuration.FallbackMessage ?? "I'm experiencing technical difficulties. Please try again later.";
        }
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey, string provider, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple validation by making a minimal API call
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.GetAsync("https://api.openai.com/v1/models", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public Task<decimal> CalculateCostAsync(string prompt, string response, AiConfiguration configuration, CancellationToken cancellationToken = default)
    {
        // Simplified cost calculation - would need actual token counting and pricing
        var promptTokens = prompt.Length / 4; // Rough approximation
        var responseTokens = response.Length / 4;
        var totalTokens = promptTokens + responseTokens;
        
        // Basic pricing (this would be configurable)
        var costPer1000Tokens = 0.002m; // Example for GPT-3.5-turbo
        var cost = (totalTokens / 1000m) * costPer1000Tokens;
        
        return Task.FromResult(cost);
    }

    public Task<bool> ShouldHandoffToOperatorAsync(string message, AiConfiguration configuration, CancellationToken cancellationToken = default)
    {
        if (!configuration.EnableHandoffToOperator)
            return Task.FromResult(false);

        // Check for handoff trigger keywords
        if (!string.IsNullOrEmpty(configuration.HandoffTriggerKeywords))
        {
            try
            {
                var keywords = JsonSerializer.Deserialize<List<string>>(configuration.HandoffTriggerKeywords) ?? new List<string>();
                var lowerMessage = message.ToLowerInvariant();
                
                foreach (var keyword in keywords)
                {
                    if (lowerMessage.Contains(keyword.ToLowerInvariant()))
                    {
                        return Task.FromResult(true);
                    }
                }
            }
            catch (JsonException)
            {
                // Invalid JSON in keywords, skip
            }
        }

        return Task.FromResult(false);
    }

    public Task<List<FaqEntry>> SuggestFaqsAsync(string message, List<FaqEntry> faqs, CancellationToken cancellationToken = default)
    {
        // Simple keyword-based FAQ matching
        var lowerMessage = message.ToLowerInvariant();
        var suggestions = new List<FaqEntry>();

        foreach (var faq in faqs.Where(f => f.IsActive && f.EnableAutoSuggestion))
        {
            var questionWords = faq.Question.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var matchCount = questionWords.Count(word => lowerMessage.Contains(word));
            
            if (matchCount > 0)
            {
                suggestions.Add(faq);
            }
        }

        return Task.FromResult(suggestions.Take(3).ToList()); // Return top 3 suggestions
    }

    public Task<string> ExtractSentimentAsync(string message, CancellationToken cancellationToken = default)
    {
        // Simplified sentiment analysis - in production, this would use a proper service
        var lowerMessage = message.ToLowerInvariant();
        
        var positiveWords = new[] { "good", "great", "excellent", "happy", "satisfied", "thank", "awesome", "wonderful" };
        var negativeWords = new[] { "bad", "terrible", "awful", "angry", "frustrated", "disappointed", "hate", "horrible" };
        
        var positiveCount = positiveWords.Count(word => lowerMessage.Contains(word));
        var negativeCount = negativeWords.Count(word => lowerMessage.Contains(word));
        
        if (positiveCount > negativeCount)
            return Task.FromResult("positive");
        else if (negativeCount > positiveCount)
            return Task.FromResult("negative");
        else
            return Task.FromResult("neutral");
    }

    public Task<string> TranslateMessageAsync(string message, string fromLanguage, string toLanguage, CancellationToken cancellationToken = default)
    {
        // Placeholder for translation service integration
        // In production, this would integrate with a translation service
        return Task.FromResult(message);
    }
}

// OpenAI API response models
public class OpenAiChatResponse
{
    public List<OpenAiChoice>? Choices { get; set; }
}

public class OpenAiChoice
{
    public OpenAiMessage? Message { get; set; }
}

public class OpenAiMessage
{
    public string? Content { get; set; }
} 