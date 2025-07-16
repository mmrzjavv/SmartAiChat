using System;
using System.Collections.Generic;

namespace SmartAiChat.Infrastructure.Services.SentimentAnalysis
{
    public class SentimentAnalysisService
    {
        private readonly Dictionary<string, int> _wordScores;

        public SentimentAnalysisService()
        {
            // A simple sentiment dictionary. In a real-world scenario, this would be
            // a pre-trained model or a more extensive dictionary.
            _wordScores = new Dictionary<string, int>
            {
                { "good", 1 }, { "great", 2 }, { "excellent", 3 }, { "happy", 2 },
                { "satisfied", 2 }, { "thank", 1 }, { "awesome", 3 }, { "wonderful", 3 },
                { "bad", -1 }, { "terrible", -2 }, { "awful", -3 }, { "angry", -2 },
                { "frustrated", -2 }, { "disappointed", -2 }, { "hate", -3 }, { "horrible", -3 }
            };
        }

        public string AnalyzeSentiment(string text)
        {
            var score = 0;
            var words = text.ToLowerInvariant().Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (_wordScores.ContainsKey(word))
                {
                    score += _wordScores[word];
                }
            }

            if (score > 0)
            {
                return "positive";
            }
            else if (score < 0)
            {
                return "negative";
            }
            else
            {
                return "neutral";
            }
        }
    }
}
