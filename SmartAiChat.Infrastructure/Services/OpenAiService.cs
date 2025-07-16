using Microsoft.Extensions.Logging;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Infrastructure.Services.SentimentAnalysis;
using SmartAiChat.Infrastructure.Services.Translation;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tiktoken;

namespace SmartAiChat.Infrastructure.Services
{
    public class OpenAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenAiService> _logger;
        private readonly SentimentAnalysisService _sentimentAnalysisService;
        private readonly TranslationService _translationService;

        public OpenAiService(HttpClient httpClient, ILogger<OpenAiService> logger, SentimentAnalysisService sentimentAnalysisService, TranslationService translationService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _sentimentAnalysisService = sentimentAnalysisService;
            _translationService = translationService;
        }

        public async Task<string> GenerateResponseAsync(string prompt, AiConfiguration configuration, List<ChatMessage>? conversationHistory = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!configuration.IsEnabled)
                {
                    return configuration.FallbackMessage ?? "AI is currently disabled. Please contact support.";
                }

                var messages = new List<object>
                {
                    new { role = "system", content = configuration.SystemPrompt }
                };

                if (configuration.EnableContextHistory && conversationHistory != null)
                {
                    var recentMessages = conversationHistory
                        .OrderBy(m => m.CreatedAt)
                        .TakeLast(configuration.ContextHistoryLimit)
                        .ToList();

                    foreach (var msg in recentMessages)
                    {
                        messages.Add(new { role = msg.IsFromAi ? "assistant" : "user", content = msg.Content });
                    }
                }

                messages.Add(new { role = "user", content = prompt });

                var requestBody = new
                {
                    model = configuration.ModelName,
                    messages = messages,
                    max_tokens = configuration.MaxTokens,
                    temperature = (float)configuration.Temperature
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                if (!string.IsNullOrEmpty(configuration.ApiKey))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", configuration.ApiKey);
                }

                var endpoint = configuration.ApiEndpoint ?? "https://api.openai.com/v1/chat/completions";
                var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    var openAiResponse = JsonSerializer.Deserialize<OpenAiChatResponse>(responseContent);

                    var assistantMessage = openAiResponse?.Choices?.FirstOrDefault()?.Message?.Content ??
                        configuration.FallbackMessage ?? "I'm sorry, I couldn't generate a response.";

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
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API: Network error.");
                return "I'm sorry, but I'm having trouble connecting to the AI service. Please check your network connection and try again.";
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API: Invalid response format.");
                return "I'm sorry, but I received an invalid response from the AI service. Please try again later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while calling the OpenAI API.");
                return configuration.FallbackMessage ?? "I'm experiencing technical difficulties. Please try again later.";
            }
        }

        public async Task<bool> ValidateApiKeyAsync(string apiKey, string provider, CancellationToken cancellationToken = default)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
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
            var encoding = Tiktoken.Encoding.ForModel(configuration.ModelName);
            var promptTokens = encoding.Encode(prompt).Count;
            var responseTokens = encoding.Encode(response).Count;
            var inputCost = (promptTokens / 1000m) * configuration.InputCostPer1000Tokens;
            var outputCost = (responseTokens / 1000m) * configuration.OutputCostPer1000Tokens;
            var totalCost = inputCost + outputCost;
            return Task.FromResult(totalCost);
        }

        public Task<bool> ShouldHandoffToOperatorAsync(string message, AiConfiguration configuration, CancellationToken cancellationToken = default)
        {
            if (!configuration.EnableHandoffToOperator)
                return Task.FromResult(false);

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
                }
            }

            return Task.FromResult(false);
        }

        public Task<List<FaqEntry>> SuggestFaqsAsync(string message, List<FaqEntry> faqs, CancellationToken cancellationToken = default)
        {
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

            return Task.FromResult(suggestions.Take(3).ToList());
        }

        public Task<string> ExtractSentimentAsync(string message, CancellationToken cancellationToken = default)
        {
            var sentiment = _sentimentAnalysisService.AnalyzeSentiment(message);
            return Task.FromResult(sentiment);
        }

        public async Task<string> TranslateMessageAsync(string message, string fromLanguage, string toLanguage, CancellationToken cancellationToken = default)
        {
            return await _translationService.TranslateAsync(message, fromLanguage, toLanguage);
        }
    }

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
}
