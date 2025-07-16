using Microsoft.Extensions.Logging;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Infrastructure.Services.SentimentAnalysis;
using SmartAiChat.Infrastructure.Services.Translation;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Infrastructure.Services
{
    public class AiServiceFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OpenAiService> _openAiLogger;
        private readonly SentimentAnalysisService _sentimentAnalysisService;
        private readonly TranslationService _translationService;

        public AiServiceFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<OpenAiService> openAiLogger,
            SentimentAnalysisService sentimentAnalysisService,
            TranslationService translationService)
        {
            _httpClientFactory = httpClientFactory;
            _openAiLogger = openAiLogger;
            _sentimentAnalysisService = sentimentAnalysisService;
            _translationService = translationService;
        }

        public IAiService Create(AiProvider provider)
        {
            switch (provider)
            {
                case AiProvider.OpenAI:
                    var httpClient = _httpClientFactory.CreateClient("OpenAI");
                    return new OpenAiService(httpClient, _openAiLogger, _sentimentAnalysisService, _translationService);
                // Add other providers here
                // case AiProvider.Anthropic:
                //     return new AnthropicService(...);
                default:
                    throw new NotSupportedException($"AI provider '{provider}' is not supported.");
            }
        }
    }
}
