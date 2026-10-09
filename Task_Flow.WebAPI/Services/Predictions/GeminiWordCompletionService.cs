using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Predictions
{
    /// <summary>
    /// Google Gemini API ilə sözü tamamlayır. HttpClient DI tərəfindən verilir (typed client).
    /// </summary>
    public class GeminiWordCompletionService : IWordCompletionService
    {
        private const string Endpoint =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        private const string ApiKeyHeader = "X-goog-api-key";

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiWordCompletionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ServiceResult<object>> CompleteAsync(string prompt)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
            {
                Content = JsonContent.Create(BuildRequestBody(prompt))
            };
            request.Headers.Add(ApiKeyHeader, _configuration["GeminiApiKey"]);

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ServiceResult<object>.Failure($"Gemini API Error: {errorContent}", (int)response.StatusCode);
            }

            var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            var prediction = result?.Candidates?[0]?.Content?.Parts?[0]?.Text;

            return ServiceResult<object>.Success(new { suggestion = prediction?.Trim() });
        }

        private static object BuildRequestBody(string prompt)
        {
            return new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"Complete this word: {prompt}. Return only the single completed word, no explanation." }
                        }
                    }
                }
            };
        }
    }
}
