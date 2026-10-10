using System.Text.Json.Serialization;

namespace Task_Flow.WebAPI.Services.Predictions
{
    // Gemini generateContent cavabının istifadə olunan hissəsi
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    public class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }
    }

    public class GeminiContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiPart>? Parts { get; set; }
    }

    public class GeminiPart
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
